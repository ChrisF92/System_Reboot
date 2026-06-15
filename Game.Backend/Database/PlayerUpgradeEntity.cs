namespace Game.Backend.Database;

public sealed class PlayerUpgradeEntity
{
    public Guid PlayerUpgradeId { get; set; }

    public Guid PlayerId { get; set; }

    public required string UpgradeId { get; set; }

    public int Level { get; set; }

    public PlayerEntity? Player { get; set; }
}
