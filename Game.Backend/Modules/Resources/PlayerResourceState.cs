using Game.Backend.Modules.Players;

namespace Game.Backend.Modules.Resources;

public sealed record PlayerResourceState(
    Guid PlayerId,
    ResourceWallet Resources,
    DateTimeOffset LastResourceClaimedAtUtc);
