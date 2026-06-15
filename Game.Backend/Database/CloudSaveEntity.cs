namespace Game.Backend.Database;

public sealed class CloudSaveEntity
{
    public Guid PlayerId { get; set; }

    public int SaveVersion { get; set; }

    public required string SaveJson { get; set; }

    public string? Checksum { get; set; }

    public DateTimeOffset? ClientSavedAtUtc { get; set; }

    public DateTimeOffset SavedAtUtc { get; set; }

    public PlayerEntity? Player { get; set; }
}
