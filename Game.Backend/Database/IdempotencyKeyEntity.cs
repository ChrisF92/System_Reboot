namespace Game.Backend.Database;

public sealed class IdempotencyKeyEntity
{
    public Guid IdempotencyKeyId { get; set; }

    public Guid AccountId { get; set; }

    public Guid? PlayerId { get; set; }

    public required string Action { get; set; }

    public required string RequestId { get; set; }

    public required string RequestHash { get; set; }

    public required string ResponseJson { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public AccountEntity? Account { get; set; }

    public PlayerEntity? Player { get; set; }
}
