namespace Game.Backend.Modules.Players;

public sealed record Player(
    Guid PlayerId,
    string DisplayName,
    int Level,
    long Xp,
    ResourceWallet Resources,
    DateTimeOffset CreatedAtUtc);

public sealed record ResourceWallet(
    long Matter,
    long Energy,
    long Data,
    long CoreFragments,
    long QuantumCores);
