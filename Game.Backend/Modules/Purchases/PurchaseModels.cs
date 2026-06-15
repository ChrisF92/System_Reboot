namespace Game.Backend.Modules.Purchases;

public sealed record PurchaseProduct(
    string ProductId,
    string ProductType,
    bool IsPowerAffecting);

public sealed record ValidatedReceipt(
    string Store,
    string ProductId,
    string TransactionId,
    string ReceiptHash);

public sealed record PurchaseReceipt(
    Guid PurchaseReceiptId,
    Guid AccountId,
    Guid PlayerId,
    string Store,
    string ProductId,
    string TransactionId,
    string ReceiptHash,
    DateTimeOffset ValidatedAtUtc);

public sealed record Entitlement(
    Guid EntitlementId,
    Guid AccountId,
    string ProductId,
    Guid SourcePurchaseReceiptId,
    DateTimeOffset GrantedAtUtc);
