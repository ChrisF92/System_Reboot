namespace Game.Backend.Modules.CloudSaves;

public sealed record CloudSave(
    Guid PlayerId,
    int SaveVersion,
    string SaveJson,
    string? Checksum,
    DateTimeOffset? ClientSavedAtUtc,
    DateTimeOffset SavedAtUtc);
