namespace Game.Backend.Database;

public sealed class PlayerEntity
{
    public Guid PlayerId { get; set; }

    public required string DisplayName { get; set; }

    public int Level { get; set; }

    public long Xp { get; set; }

    public long Matter { get; set; }

    public long Energy { get; set; }

    public long Data { get; set; }

    public long CoreFragments { get; set; }

    public long QuantumCores { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}
