namespace Game.Backend.Database;

public sealed class PurchaseReceiptEntity
{
    public Guid PurchaseReceiptId { get; set; }

    public Guid AccountId { get; set; }

    public Guid PlayerId { get; set; }

    public required string Store { get; set; }

    public required string ProductId { get; set; }

    public required string TransactionId { get; set; }

    public required string ReceiptHash { get; set; }

    public DateTimeOffset ValidatedAtUtc { get; set; }

    public AccountEntity? Account { get; set; }

    public PlayerEntity? Player { get; set; }
}
