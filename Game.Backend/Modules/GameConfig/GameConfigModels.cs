namespace Game.Backend.Modules.GameConfig;

public sealed record GameConfigVersion(
    string ConfigVersion,
    string ConfigJson,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ActivatedAtUtc);
