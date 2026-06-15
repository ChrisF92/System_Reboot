namespace Game.Backend.Modules.Players;

public sealed record Player(
    Guid PlayerId,
    Guid AccountId,
    string DisplayName,
    int Level,
    long Xp,
    ResourceWallet Resources,
    TrainingStats Training,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset LastResourceClaimedAtUtc);

public sealed record ResourceWallet(
    long Matter,
    long Energy,
    long Data,
    long CoreFragments,
    long QuantumCores);

public sealed record TrainingStats(
    int Processing,
    int Integrity,
    int Output,
    int Hardening,
    int Efficiency,
    int Bandwidth);
