using Game.Backend.Modules.GameConfig;
using Game.Backend.Modules.Idempotency;
using Game.Backend.Modules.Players;
using Game.Backend.Shared.Errors;

namespace Game.Backend.Modules.Resources;

public sealed class ResourceService
{
    private readonly ResourceRepository _resourceRepository;
    private readonly PlayerService _playerService;
    private readonly GameConfigService _gameConfigService;
    private readonly IdempotencyService _idempotencyService;
    private readonly TimeProvider _timeProvider;

    public ResourceService(
        ResourceRepository resourceRepository,
        PlayerService playerService,
        GameConfigService gameConfigService,
        IdempotencyService idempotencyService,
        TimeProvider timeProvider)
    {
        _resourceRepository = resourceRepository;
        _playerService = playerService;
        _gameConfigService = gameConfigService;
        _idempotencyService = idempotencyService;
        _timeProvider = timeProvider;
    }

    public async Task<PlayerResourcesResponse> GetResourcesAsync(
        Guid accountId,
        Guid playerId,
        CancellationToken cancellationToken)
    {
        await _playerService.EnsurePlayerOwnedByAccountAsync(accountId, playerId, cancellationToken);
        var state = await GetResourceStateAsync(playerId, cancellationToken);
        return ToResourcesResponse(state);
    }

    public async Task<OfflineResourceClaimResponse> ClaimOfflineResourcesAsync(
        Guid accountId,
        Guid playerId,
        ClaimOfflineResourcesRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ValidationException(
                "missing_resource_claim_request",
                "Resource claim request is required.");
        }

        var requestId = _idempotencyService.ValidateRequestId(request.RequestId);
        var requestHash = _idempotencyService.HashPayload(new
        {
            playerId
        });
        var replay = await _idempotencyService.GetReplayAsync<OfflineResourceClaimResponse>(
            accountId,
            "resources.claim_offline",
            requestId,
            requestHash,
            cancellationToken);
        if (replay is not null)
        {
            return replay;
        }

        await _playerService.EnsurePlayerOwnedByAccountAsync(accountId, playerId, cancellationToken);

        var state = await GetResourceStateAsync(playerId, cancellationToken);
        var claimedAtUtc = _timeProvider.GetUtcNow();
        var elapsedSeconds = CalculateElapsedSeconds(state.LastResourceClaimedAtUtc, claimedAtUtc);
        var config = await _gameConfigService.GetCurrentConfigAsync(cancellationToken);
        var appliedSeconds = Math.Min(elapsedSeconds, config.OfflineProgress.EarlyOfflineCapSeconds);
        var gain = CalculateGain(config.Resources, appliedSeconds, config.OfflineProgress.BaseOfflineEfficiency);

        if (!gain.HasAnyGain)
        {
            var noGainResponse = ToClaimResponse(state, gain, elapsedSeconds, appliedSeconds, claimedAtUtc);
            await _idempotencyService.StoreResponseAsync(
                accountId,
                playerId,
                "resources.claim_offline",
                requestId,
                requestHash,
                noGainResponse,
                cancellationToken);

            return noGainResponse;
        }

        var updatedState = await _resourceRepository.AddOfflineGainsAsync(
            playerId,
            gain,
            claimedAtUtc,
            cancellationToken);

        if (updatedState is null)
        {
            throw new NotFoundException("player_not_found", "Player profile was not found.");
        }

        var response = ToClaimResponse(updatedState, gain, elapsedSeconds, appliedSeconds, claimedAtUtc);
        await _idempotencyService.StoreResponseAsync(
            accountId,
            playerId,
            "resources.claim_offline",
            requestId,
            requestHash,
            response,
            cancellationToken);

        return response;
    }

    private async Task<PlayerResourceState> GetResourceStateAsync(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        var state = await _resourceRepository.GetAsync(playerId, cancellationToken);
        if (state is null)
        {
            throw new NotFoundException("player_not_found", "Player profile was not found.");
        }

        return state;
    }

    private static int CalculateElapsedSeconds(
        DateTimeOffset lastClaimedAtUtc,
        DateTimeOffset claimedAtUtc)
    {
        var elapsed = claimedAtUtc - lastClaimedAtUtc;
        if (elapsed <= TimeSpan.Zero)
        {
            return 0;
        }

        return (int)Math.Min(elapsed.TotalSeconds, int.MaxValue);
    }

    private static ResourceGain CalculateGain(
        ResourceGenerationConfig resources,
        int appliedSeconds,
        decimal offlineEfficiency)
    {
        return new ResourceGain(
            CalculateResourceGain(resources.MatterPerSecond, appliedSeconds, offlineEfficiency),
            CalculateResourceGain(resources.EnergyPerSecond, appliedSeconds, offlineEfficiency),
            CalculateResourceGain(resources.DataPerSecond, appliedSeconds, offlineEfficiency));
    }

    private static long CalculateResourceGain(
        decimal perSecond,
        int appliedSeconds,
        decimal offlineEfficiency)
    {
        return (long)Math.Floor(perSecond * appliedSeconds * offlineEfficiency);
    }

    private static PlayerResourcesResponse ToResourcesResponse(PlayerResourceState state)
    {
        return new PlayerResourcesResponse(
            state.PlayerId,
            ToWalletResponse(state.Resources),
            state.LastResourceClaimedAtUtc);
    }

    private static OfflineResourceClaimResponse ToClaimResponse(
        PlayerResourceState state,
        ResourceGain gain,
        int elapsedSeconds,
        int appliedSeconds,
        DateTimeOffset claimedAtUtc)
    {
        return new OfflineResourceClaimResponse(
            state.PlayerId,
            ToWalletResponse(state.Resources),
            new ResourceGainResponse(gain.Matter, gain.Energy, gain.Data),
            elapsedSeconds,
            appliedSeconds,
            state.LastResourceClaimedAtUtc,
            claimedAtUtc);
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
