namespace Game.Backend.Database;

public sealed class EntitlementEntity
{
    public Guid EntitlementId { get; set; }

    public Guid AccountId { get; set; }

    public required string ProductId { get; set; }

    public Guid SourcePurchaseReceiptId { get; set; }

    public DateTimeOffset GrantedAtUtc { get; set; }

    public AccountEntity? Account { get; set; }

    public PurchaseReceiptEntity? SourcePurchaseReceipt { get; set; }
}
