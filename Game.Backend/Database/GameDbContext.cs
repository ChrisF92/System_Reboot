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

    public DbSet<PurchaseReceiptEntity> PurchaseReceipts => Set<PurchaseReceiptEntity>();

    public DbSet<EntitlementEntity> Entitlements => Set<EntitlementEntity>();

    public DbSet<IdempotencyKeyEntity> IdempotencyKeys => Set<IdempotencyKeyEntity>();

    public DbSet<GameConfigVersionEntity> GameConfigVersions => Set<GameConfigVersionEntity>();

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
            session.Property(entity => entity.RevokedAtUtc);

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
            player.Property(entity => entity.Processing)
                .IsRequired();
            player.Property(entity => entity.Integrity)
                .IsRequired();
            player.Property(entity => entity.Output)
                .IsRequired();
            player.Property(entity => entity.Hardening)
                .IsRequired();
            player.Property(entity => entity.Efficiency)
                .IsRequired();
            player.Property(entity => entity.Bandwidth)
                .IsRequired();
            player.Property(entity => entity.CreatedAtUtc)
                .IsRequired();
            player.Property(entity => entity.LastResourceClaimedAtUtc)
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

        modelBuilder.Entity<PurchaseReceiptEntity>(purchaseReceipt =>
        {
            purchaseReceipt.ToTable("PurchaseReceipts");
            purchaseReceipt.HasKey(entity => entity.PurchaseReceiptId);
            purchaseReceipt.Property(entity => entity.Store)
                .HasMaxLength(32)
                .IsRequired();
            purchaseReceipt.Property(entity => entity.ProductId)
                .HasMaxLength(64)
                .IsRequired();
            purchaseReceipt.Property(entity => entity.TransactionId)
                .HasMaxLength(128)
                .IsRequired();
            purchaseReceipt.Property(entity => entity.ReceiptHash)
                .HasMaxLength(128)
                .IsRequired();
            purchaseReceipt.HasIndex(entity => entity.ReceiptHash)
                .IsUnique();
            purchaseReceipt.Property(entity => entity.ValidatedAtUtc)
                .IsRequired();

            purchaseReceipt.HasOne(entity => entity.Account)
                .WithMany()
                .HasForeignKey(entity => entity.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            purchaseReceipt.HasOne(entity => entity.Player)
                .WithMany()
                .HasForeignKey(entity => entity.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EntitlementEntity>(entitlement =>
        {
            entitlement.ToTable("Entitlements");
            entitlement.HasKey(entity => entity.EntitlementId);
            entitlement.Property(entity => entity.ProductId)
                .HasMaxLength(64)
                .IsRequired();
            entitlement.Property(entity => entity.GrantedAtUtc)
                .IsRequired();
            entitlement.HasIndex(entity => new { entity.AccountId, entity.ProductId })
                .IsUnique();

            entitlement.HasOne(entity => entity.Account)
                .WithMany()
                .HasForeignKey(entity => entity.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            entitlement.HasOne(entity => entity.SourcePurchaseReceipt)
                .WithMany()
                .HasForeignKey(entity => entity.SourcePurchaseReceiptId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IdempotencyKeyEntity>(idempotencyKey =>
        {
            idempotencyKey.ToTable("IdempotencyKeys");
            idempotencyKey.HasKey(entity => entity.IdempotencyKeyId);
            idempotencyKey.Property(entity => entity.Action)
                .HasMaxLength(64)
                .IsRequired();
            idempotencyKey.Property(entity => entity.RequestId)
                .HasMaxLength(96)
                .IsRequired();
            idempotencyKey.Property(entity => entity.RequestHash)
                .HasMaxLength(128)
                .IsRequired();
            idempotencyKey.Property(entity => entity.ResponseJson)
                .HasMaxLength(32_768)
                .IsRequired();
            idempotencyKey.Property(entity => entity.CreatedAtUtc)
                .IsRequired();
            idempotencyKey.HasIndex(entity => new { entity.AccountId, entity.Action, entity.RequestId })
                .IsUnique();

            idempotencyKey.HasOne(entity => entity.Account)
                .WithMany()
                .HasForeignKey(entity => entity.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            idempotencyKey.HasOne(entity => entity.Player)
                .WithMany()
                .HasForeignKey(entity => entity.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GameConfigVersionEntity>(gameConfigVersion =>
        {
            gameConfigVersion.ToTable("GameConfigVersions");
            gameConfigVersion.HasKey(entity => entity.ConfigVersion);
            gameConfigVersion.Property(entity => entity.ConfigVersion)
                .HasMaxLength(64)
                .IsRequired();
            gameConfigVersion.Property(entity => entity.ConfigJson)
                .HasMaxLength(32_768)
                .IsRequired();
            gameConfigVersion.Property(entity => entity.IsActive)
                .IsRequired();
            gameConfigVersion.Property(entity => entity.CreatedAtUtc)
                .IsRequired();
            gameConfigVersion.HasIndex(entity => entity.IsActive);
        });
    }
}
