namespace Game.Backend.Database;

public sealed class GameConfigVersionEntity
{
    public required string ConfigVersion { get; set; }

    public required string ConfigJson { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? ActivatedAtUtc { get; set; }
}
