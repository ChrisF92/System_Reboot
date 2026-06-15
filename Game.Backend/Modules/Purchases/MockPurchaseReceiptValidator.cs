using Game.Backend.Modules.Auth;
using Game.Backend.Shared.Errors;

namespace Game.Backend.Modules.Purchases;

public sealed class MockPurchaseReceiptValidator
{
    public const string LocalMockStore = "local_mock";

    private readonly TokenHasher _tokenHasher;

    public MockPurchaseReceiptValidator(TokenHasher tokenHasher)
    {
        _tokenHasher = tokenHasher;
    }

    public ValidatedReceipt Validate(
        string store,
        string productId,
        string receipt)
    {
        if (!string.Equals(store, LocalMockStore, StringComparison.Ordinal))
        {
            throw new ValidationException(
                "unsupported_store",
                "Only local mock receipts are supported in this backend slice.");
        }

        var parts = receipt.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length != 3 ||
            !string.Equals(parts[0], "local", StringComparison.Ordinal) ||
            !string.Equals(parts[1], productId, StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(parts[2]))
        {
            throw new ValidationException(
                "invalid_receipt",
                "Receipt must use local:<productId>:<transactionId> format.");
        }

        return new ValidatedReceipt(
            store,
            productId,
            parts[2],
            _tokenHasher.HashToken(receipt));
    }
}
