using Game.Backend.Modules.Players;

namespace Game.Backend.Modules.Resources;

public sealed record ClaimOfflineResourcesRequest(string? RequestId);

public sealed record PlayerResourcesResponse(
    Guid PlayerId,
    ResourceWalletResponse Resources,
    DateTimeOffset LastResourceClaimedAtUtc);

public sealed record OfflineResourceClaimResponse(
    Guid PlayerId,
    ResourceWalletResponse Resources,
    ResourceGainResponse Gains,
    int ElapsedSeconds,
    int AppliedSeconds,
    DateTimeOffset LastResourceClaimedAtUtc,
    DateTimeOffset ClaimedAtUtc);

public sealed record ResourceGainResponse(
    long Matter,
    long Energy,
    long Data);
