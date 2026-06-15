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

    public DbSet<AccountEntity> Accounts => Set<AccountEntity>();

    public DbSet<AccountSessionEntity> AccountSessions => Set<AccountSessionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountEntity>(account =>
        {
            account.ToTable("Accounts");
            account.HasKey(entity => entity.AccountId);
            account.Property(entity => entity.Email)
                .HasMaxLength(254)
                .IsRequired();
            account.Property(entity => entity.NormalizedEmail)
                .HasMaxLength(254)
                .IsRequired();
            account.HasIndex(entity => entity.NormalizedEmail)
                .IsUnique();
            account.Property(entity => entity.PasswordHash)
                .HasMaxLength(128)
                .IsRequired();
            account.Property(entity => entity.PasswordSalt)
                .HasMaxLength(128)
                .IsRequired();
            account.Property(entity => entity.PasswordIterations)
                .IsRequired();
            account.Property(entity => entity.CreatedAtUtc)
                .IsRequired();
        });

        modelBuilder.Entity<AccountSessionEntity>(session =>
        {
            session.ToTable("AccountSessions");
            session.HasKey(entity => entity.SessionId);
            session.Property(entity => entity.TokenHash)
                .HasMaxLength(128)
                .IsRequired();
            session.HasIndex(entity => entity.TokenHash)
                .IsUnique();
            session.Property(entity => entity.CreatedAtUtc)
                .IsRequired();
            session.Property(entity => entity.ExpiresAtUtc)
                .IsRequired();

            session.HasOne(entity => entity.Account)
                .WithMany()
                .HasForeignKey(entity => entity.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PlayerEntity>(player =>
        {
            player.ToTable("Players");
            player.HasKey(entity => entity.PlayerId);
            player.Property(entity => entity.AccountId)
                .IsRequired();
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

            player.HasOne(entity => entity.Account)
                .WithMany()
                .HasForeignKey(entity => entity.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
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
