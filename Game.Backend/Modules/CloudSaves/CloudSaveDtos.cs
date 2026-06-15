namespace Game.Backend.Modules.CloudSaves;

public sealed record UpsertCloudSaveRequest(
    int SaveVersion,
    string? SaveJson,
    string? Checksum,
    DateTimeOffset? ClientSavedAtUtc);

public sealed record CloudSaveResponse(
    Guid PlayerId,
    int SaveVersion,
    string SaveJson,
    string? Checksum,
    DateTimeOffset? ClientSavedAtUtc,
    DateTimeOffset SavedAtUtc);
