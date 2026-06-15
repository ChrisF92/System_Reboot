using Game.Backend.Modules.GameConfig;
using Game.Backend.Modules.Idempotency;
using Game.Backend.Modules.Players;
using Game.Backend.Shared.Errors;

namespace Game.Backend.Modules.Upgrades;

public sealed class UpgradeService
{
    private readonly UpgradeRepository _upgradeRepository;
    private readonly PlayerService _playerService;
    private readonly GameConfigService _gameConfigService;
    private readonly IdempotencyService _idempotencyService;

    public UpgradeService(
        UpgradeRepository upgradeRepository,
        PlayerService playerService,
        GameConfigService gameConfigService,
        IdempotencyService idempotencyService)
    {
        _upgradeRepository = upgradeRepository;
        _playerService = playerService;
        _gameConfigService = gameConfigService;
        _idempotencyService = idempotencyService;
    }

    public async Task<PlayerUpgradesResponse> GetUpgradesAsync(
        Guid accountId,
        Guid playerId,
        CancellationToken cancellationToken)
    {
        await _playerService.EnsurePlayerOwnedByAccountAsync(accountId, playerId, cancellationToken);
        var state = await GetUpgradeStateAsync(playerId, cancellationToken);
        var config = await _gameConfigService.GetCurrentConfigAsync(cancellationToken);
        return ToUpgradesResponse(state, config.Upgrades);
    }

    public async Task<PurchaseUpgradeResponse> PurchaseUpgradeAsync(
        Guid accountId,
        Guid playerId,
        string upgradeId,
        PurchaseUpgradeRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ValidationException("missing_upgrade_request", "Upgrade purchase request is required.");
        }

        var normalizedUpgradeId = upgradeId.Trim().ToLowerInvariant();
        var requestId = _idempotencyService.ValidateRequestId(request.RequestId);
        var requestHash = _idempotencyService.HashPayload(new
        {
            playerId,
            upgradeId = normalizedUpgradeId
        });
        var replay = await _idempotencyService.GetReplayAsync<PurchaseUpgradeResponse>(
            accountId,
            "upgrades.purchase",
            requestId,
            requestHash,
            cancellationToken);
        if (replay is not null)
        {
            return replay;
        }

        await _playerService.EnsurePlayerOwnedByAccountAsync(accountId, playerId, cancellationToken);

        var state = await GetUpgradeStateAsync(playerId, cancellationToken);
        var config = await _gameConfigService.GetCurrentConfigAsync(cancellationToken);
        var definition = GetDefinition(config.Upgrades, normalizedUpgradeId);
        var currentLevel = GetLevel(state, normalizedUpgradeId);
        var cost = CalculateCost(definition, currentLevel);

        if (!cost.CanPay(state.Resources))
        {
            throw new ValidationException("insufficient_resources", "Not enough resources to purchase this upgrade.");
        }

        var updatedState = await _upgradeRepository.PurchaseAsync(
            playerId,
            normalizedUpgradeId,
            cost,
            cancellationToken);

        if (updatedState is null)
        {
            throw new NotFoundException("player_not_found", "Player profile was not found.");
        }

        var response = new PurchaseUpgradeResponse(
            updatedState.PlayerId,
            ToUpgradeResponse(definition, GetLevel(updatedState, normalizedUpgradeId)),
            cost.ToResponse(),
            ToWalletResponse(updatedState.Resources));

        await _idempotencyService.StoreResponseAsync(
            accountId,
            playerId,
            "upgrades.purchase",
            requestId,
            requestHash,
            response,
            cancellationToken);

        return response;
    }

    private async Task<UpgradeState> GetUpgradeStateAsync(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        var state = await _upgradeRepository.GetAsync(playerId, cancellationToken);
        if (state is null)
        {
            throw new NotFoundException("player_not_found", "Player profile was not found.");
        }

        return state;
    }

    private static PlayerUpgradesResponse ToUpgradesResponse(
        UpgradeState state,
        UpgradeConfig upgradeConfig)
    {
        return new PlayerUpgradesResponse(
            state.PlayerId,
            upgradeConfig.Items
                .Select(definition => ToUpgradeResponse(definition, GetLevel(state, definition.UpgradeId)))
                .ToArray(),
            ToWalletResponse(state.Resources));
    }

    private static UpgradeDefinitionResponse GetDefinition(UpgradeConfig config, string upgradeId)
    {
        var definition = config.Items.SingleOrDefault(
            upgrade => string.Equals(upgrade.UpgradeId, upgradeId, StringComparison.Ordinal));

        if (definition is null)
        {
            throw new ValidationException("unknown_upgrade", "Upgrade id is not recognized.");
        }

        return definition;
    }

    private static int GetLevel(UpgradeState state, string upgradeId)
    {
        return state.UpgradeLevels.TryGetValue(upgradeId, out var level) ? level : 0;
    }

    private static UpgradeCost CalculateCost(UpgradeDefinitionResponse definition, int currentLevel)
    {
        var multiplier = currentLevel + 1;
        return new UpgradeCost(
            definition.MatterCost * multiplier,
            definition.EnergyCost * multiplier,
            definition.DataCost * multiplier);
    }

    private static PlayerUpgradeResponse ToUpgradeResponse(
        UpgradeDefinitionResponse definition,
        int level)
    {
        var nextCost = CalculateCost(definition, level);
        return new PlayerUpgradeResponse(
            definition.UpgradeId,
            definition.DisplayName,
            definition.Description,
            level,
            nextCost.ToResponse(),
            new UpgradeEffectResponse(
                definition.EffectType,
                definition.EffectValuePerLevel * level));
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
