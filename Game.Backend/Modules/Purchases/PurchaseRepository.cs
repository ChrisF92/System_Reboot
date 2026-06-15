using Game.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.Purchases;

public sealed class PurchaseRepository
{
    private readonly GameDbContext _dbContext;

    public PurchaseRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ReceiptExistsAsync(
        string receiptHash,
        CancellationToken cancellationToken)
    {
        return await _dbContext.PurchaseReceipts
            .AsNoTracking()
            .AnyAsync(receipt => receipt.ReceiptHash == receiptHash, cancellationToken);
    }

    public async Task<(PurchaseReceipt Receipt, Entitlement Entitlement)> AddValidatedPurchaseAsync(
        PurchaseReceipt receipt,
        Entitlement entitlement,
        CancellationToken cancellationToken)
    {
        _dbContext.PurchaseReceipts.Add(ToEntity(receipt));
        _dbContext.Entitlements.Add(ToEntity(entitlement));
        await _dbContext.SaveChangesAsync(cancellationToken);
        return (receipt, entitlement);
    }

    private static PurchaseReceiptEntity ToEntity(PurchaseReceipt receipt)
    {
        return new PurchaseReceiptEntity
        {
            PurchaseReceiptId = receipt.PurchaseReceiptId,
            AccountId = receipt.AccountId,
            PlayerId = receipt.PlayerId,
            Store = receipt.Store,
            ProductId = receipt.ProductId,
            TransactionId = receipt.TransactionId,
            ReceiptHash = receipt.ReceiptHash,
            ValidatedAtUtc = receipt.ValidatedAtUtc
        };
    }

    private static EntitlementEntity ToEntity(Entitlement entitlement)
    {
        return new EntitlementEntity
        {
            EntitlementId = entitlement.EntitlementId,
            AccountId = entitlement.AccountId,
            ProductId = entitlement.ProductId,
            SourcePurchaseReceiptId = entitlement.SourcePurchaseReceiptId,
            GrantedAtUtc = entitlement.GrantedAtUtc
        };
    }
}
