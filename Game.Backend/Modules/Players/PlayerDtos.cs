namespace Game.Backend.Modules.Players;

public sealed record CreatePlayerRequest(string? DisplayName);

public sealed record PlayerProfileResponse(
    Guid PlayerId,
    string DisplayName,
    int Level,
    long Xp,
    ResourceWalletResponse Resources,
    DateTimeOffset CreatedAtUtc);

public sealed record ResourceWalletResponse(
    long Matter,
    long Energy,
    long Data,
    long CoreFragments,
    long QuantumCores);
