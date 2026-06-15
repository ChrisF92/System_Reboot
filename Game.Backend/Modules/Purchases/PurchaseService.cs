using Game.Backend.Modules.Players;
using Game.Backend.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.Purchases;

public sealed class PurchaseService
{
    private const int MaximumStoreLength = 32;
    private const int MaximumProductIdLength = 64;
    private const int MaximumReceiptLength = 512;

    private readonly PurchaseRepository _purchaseRepository;
    private readonly PurchaseProductCatalog _productCatalog;
    private readonly MockPurchaseReceiptValidator _receiptValidator;
    private readonly PlayerService _playerService;
    private readonly TimeProvider _timeProvider;

    public PurchaseService(
        PurchaseRepository purchaseRepository,
        PurchaseProductCatalog productCatalog,
        MockPurchaseReceiptValidator receiptValidator,
        PlayerService playerService,
        TimeProvider timeProvider)
    {
        _purchaseRepository = purchaseRepository;
        _productCatalog = productCatalog;
        _receiptValidator = receiptValidator;
        _playerService = playerService;
        _timeProvider = timeProvider;
    }

    public async Task<ValidatePurchaseResponse> ValidatePurchaseAsync(
        Guid accountId,
        ValidatePurchaseRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ValidationException("missing_purchase_request", "Purchase validation request is required.");
        }

        await _playerService.EnsurePlayerOwnedByAccountAsync(
            accountId,
            request.PlayerId,
            cancellationToken);

        var store = ValidateRequiredString(request.Store, "store_required", "Store is required.", MaximumStoreLength);
        var productId = ValidateRequiredString(
            request.ProductId,
            "product_id_required",
            "Product id is required.",
            MaximumProductIdLength);
        var receipt = ValidateRequiredString(
            request.Receipt,
            "receipt_required",
            "Receipt is required.",
            MaximumReceiptLength);

        var product = _productCatalog.GetProduct(productId);
        if (product is null)
        {
            throw new ValidationException("unknown_product", "Product id is not recognized.");
        }

        if (product.IsPowerAffecting)
        {
            throw new ValidationException(
                "power_product_not_allowed",
                "Power-affecting products are not allowed.");
        }

        var validatedReceipt = _receiptValidator.Validate(store, productId, receipt);
        if (await _purchaseRepository.ReceiptExistsAsync(validatedReceipt.ReceiptHash, cancellationToken))
        {
            throw new ConflictException("duplicate_receipt", "Receipt has already been validated.");
        }

        var now = _timeProvider.GetUtcNow();
        var purchaseReceipt = new PurchaseReceipt(
            Guid.NewGuid(),
            accountId,
            request.PlayerId,
            validatedReceipt.Store,
            validatedReceipt.ProductId,
            validatedReceipt.TransactionId,
            validatedReceipt.ReceiptHash,
            now);
        var entitlement = new Entitlement(
            Guid.NewGuid(),
            accountId,
            product.ProductId,
            purchaseReceipt.PurchaseReceiptId,
            now);

        try
        {
            var storedPurchase = await _purchaseRepository.AddValidatedPurchaseAsync(
                purchaseReceipt,
                entitlement,
                cancellationToken);

            return new ValidatePurchaseResponse(
                storedPurchase.Receipt.PurchaseReceiptId,
                storedPurchase.Entitlement.EntitlementId,
                accountId,
                request.PlayerId,
                validatedReceipt.Store,
                product.ProductId,
                product.ProductType,
                storedPurchase.Receipt.ValidatedAtUtc,
                storedPurchase.Entitlement.GrantedAtUtc);
        }
        catch (DbUpdateException)
        {
            throw new ConflictException(
                "purchase_already_granted",
                "Purchase has already been granted for this account.");
        }
    }

    private static string ValidateRequiredString(
        string? value,
        string errorCode,
        string errorMessage,
        int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException(errorCode, errorMessage);
        }

        var trimmed = value.Trim();
        if (trimmed.Length > maximumLength)
        {
            throw new ValidationException(
                $"{errorCode}_too_long",
                $"{errorMessage.TrimEnd('.')} must be no more than {maximumLength} characters.");
        }

        return trimmed;
    }
}
