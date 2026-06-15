using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Database;

public sealed class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    public DbSet<PlayerEntity> Players => Set<PlayerEntity>();

    public DbSet<CloudSaveEntity> CloudSaves => Set<CloudSaveEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerEntity>(player =>
        {
            player.ToTable("Players");
            player.HasKey(entity => entity.PlayerId);
            player.Property(entity => entity.DisplayName)
                .HasMaxLength(24)
                .IsRequired();
            player.Property(entity => entity.Level)
                .IsRequired();
            player.Property(entity => entity.Xp)
                .IsRequired();
            player.Property(entity => entity.Matter)
                .IsRequired();
            player.Property(entity => entity.Energy)
                .IsRequired();
            player.Property(entity => entity.Data)
                .IsRequired();
            player.Property(entity => entity.CoreFragments)
                .IsRequired();
            player.Property(entity => entity.QuantumCores)
                .IsRequired();
            player.Property(entity => entity.CreatedAtUtc)
                .IsRequired();
        });

        modelBuilder.Entity<CloudSaveEntity>(cloudSave =>
        {
            cloudSave.ToTable("CloudSaves");
            cloudSave.HasKey(entity => entity.PlayerId);
            cloudSave.Property(entity => entity.SaveVersion)
                .IsRequired();
            cloudSave.Property(entity => entity.SaveJson)
                .HasMaxLength(131_072)
                .IsRequired();
            cloudSave.Property(entity => entity.Checksum)
                .HasMaxLength(128);
            cloudSave.Property(entity => entity.SavedAtUtc)
                .IsRequired();

            cloudSave.HasOne(entity => entity.Player)
                .WithOne()
                .HasForeignKey<CloudSaveEntity>(entity => entity.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
