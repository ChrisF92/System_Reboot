namespace Game.Backend.Modules.Idempotency;

public sealed record IdempotencyKey(
    Guid IdempotencyKeyId,
    Guid AccountId,
    Guid? PlayerId,
    string Action,
    string RequestId,
    string RequestHash,
    string ResponseJson,
    DateTimeOffset CreatedAtUtc);
