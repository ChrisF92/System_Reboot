using Game.Backend.Modules.GameConfig;
using Game.Backend.Modules.Idempotency;
using Game.Backend.Modules.Players;
using Game.Backend.Shared.Errors;

namespace Game.Backend.Modules.Training;

public sealed class TrainingService
{
    private readonly TrainingRepository _trainingRepository;
    private readonly PlayerService _playerService;
    private readonly GameConfigService _gameConfigService;
    private readonly IdempotencyService _idempotencyService;

    public TrainingService(
        TrainingRepository trainingRepository,
        PlayerService playerService,
        GameConfigService gameConfigService,
        IdempotencyService idempotencyService)
    {
        _trainingRepository = trainingRepository;
        _playerService = playerService;
        _gameConfigService = gameConfigService;
        _idempotencyService = idempotencyService;
    }

    public async Task<TrainingResponse> GetTrainingAsync(
        Guid accountId,
        Guid playerId,
        CancellationToken cancellationToken)
    {
        await _playerService.EnsurePlayerOwnedByAccountAsync(accountId, playerId, cancellationToken);
        var state = await GetTrainingStateAsync(playerId, cancellationToken);
        var config = await _gameConfigService.GetCurrentConfigAsync(cancellationToken);
        return ToTrainingResponse(state, config.Training);
    }

    public async Task<UpgradeTrainingResponse> UpgradeTrainingAsync(
        Guid accountId,
        Guid playerId,
        string statId,
        UpgradeTrainingRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ValidationException("missing_training_request", "Training upgrade request is required.");
        }

        var normalizedStatId = statId.Trim().ToLowerInvariant();
        var requestId = _idempotencyService.ValidateRequestId(request.RequestId);
        var requestHash = _idempotencyService.HashPayload(new
        {
            playerId,
            statId = normalizedStatId
        });
        var replay = await _idempotencyService.GetReplayAsync<UpgradeTrainingResponse>(
            accountId,
            "training.upgrade",
            requestId,
            requestHash,
            cancellationToken);
        if (replay is not null)
        {
            return replay;
        }

        await _playerService.EnsurePlayerOwnedByAccountAsync(accountId, playerId, cancellationToken);

        var state = await GetTrainingStateAsync(playerId, cancellationToken);
        var config = await _gameConfigService.GetCurrentConfigAsync(cancellationToken);
        var definition = GetDefinition(config.Training, normalizedStatId);
        var currentLevel = GetStatLevel(state.Stats, normalizedStatId);
        var cost = CalculateCost(definition, currentLevel);

        if (!cost.CanPay(state.Resources))
        {
            throw new ValidationException("insufficient_resources", "Not enough resources to train this stat.");
        }

        var updatedState = await _trainingRepository.UpgradeAsync(
            playerId,
            normalizedStatId,
            cost,
            cancellationToken);

        if (updatedState is null)
        {
            throw new NotFoundException("player_not_found", "Player profile was not found.");
        }

        var response = new UpgradeTrainingResponse(
            updatedState.PlayerId,
            ToStatResponse(definition, GetStatLevel(updatedState.Stats, normalizedStatId)),
            new TrainingCostResponse(cost.Matter, cost.Energy, cost.Data),
            ToWalletResponse(updatedState.Resources));

        await _idempotencyService.StoreResponseAsync(
            accountId,
            playerId,
            "training.upgrade",
            requestId,
            requestHash,
            response,
            cancellationToken);

        return response;
    }

    private async Task<TrainingState> GetTrainingStateAsync(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        var state = await _trainingRepository.GetAsync(playerId, cancellationToken);
        if (state is null)
        {
            throw new NotFoundException("player_not_found", "Player profile was not found.");
        }

        return state;
    }

    private static TrainingResponse ToTrainingResponse(
        TrainingState state,
        TrainingConfig trainingConfig)
    {
        return new TrainingResponse(
            state.PlayerId,
            trainingConfig.Stats
                .Select(definition => ToStatResponse(definition, GetStatLevel(state.Stats, definition.StatId)))
                .ToArray(),
            ToWalletResponse(state.Resources));
    }

    private static TrainingStatDefinitionResponse GetDefinition(
        TrainingConfig trainingConfig,
        string statId)
    {
        var definition = trainingConfig.Stats.SingleOrDefault(
            stat => string.Equals(stat.StatId, statId, StringComparison.Ordinal));

        if (definition is null)
        {
            throw new ValidationException("unknown_training_stat", "Training stat id is not recognized.");
        }

        return definition;
    }

    private static TrainingCost CalculateCost(
        TrainingStatDefinitionResponse definition,
        int currentLevel)
    {
        return new TrainingCost(
            definition.MatterCost * currentLevel,
            definition.EnergyCost * currentLevel,
            definition.DataCost * currentLevel);
    }

    private static TrainingStatResponse ToStatResponse(
        TrainingStatDefinitionResponse definition,
        int level)
    {
        var nextCost = CalculateCost(definition, level);
        return new TrainingStatResponse(
            definition.StatId,
            definition.DisplayName,
            level,
            new TrainingCostResponse(nextCost.Matter, nextCost.Energy, nextCost.Data));
    }

    private static int GetStatLevel(TrainingStats stats, string statId)
    {
        return statId switch
        {
            TrainingStatIds.Processing => stats.Processing,
            TrainingStatIds.Integrity => stats.Integrity,
            TrainingStatIds.Output => stats.Output,
            TrainingStatIds.Hardening => stats.Hardening,
            TrainingStatIds.Efficiency => stats.Efficiency,
            TrainingStatIds.Bandwidth => stats.Bandwidth,
            _ => throw new ValidationException("unknown_training_stat", "Training stat id is not recognized.")
        };
    }

    private static ResourceWalletResponse ToWalletResponse(ResourceWallet wallet)
    {
        return new ResourceWalletResponse(
            wallet.Matter,
            wallet.Energy,
            wallet.Data,
            wallet.CoreFragments,
            wallet.QuantumCores);
    }
}
