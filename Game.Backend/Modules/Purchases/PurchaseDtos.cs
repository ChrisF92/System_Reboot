namespace Game.Backend.Modules.Purchases;

public sealed record ValidatePurchaseRequest(
    string? RequestId,
    Guid PlayerId,
    string? Store,
    string? ProductId,
    string? Receipt);

public sealed record ValidatePurchaseResponse(
    Guid PurchaseReceiptId,
    Guid EntitlementId,
    Guid AccountId,
    Guid PlayerId,
    string Store,
    string ProductId,
    string ProductType,
    DateTimeOffset ValidatedAtUtc,
    DateTimeOffset GrantedAtUtc);
