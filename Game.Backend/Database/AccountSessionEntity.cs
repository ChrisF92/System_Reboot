namespace Game.Backend.Database;

public sealed class AccountSessionEntity
{
    public Guid SessionId { get; set; }

    public Guid AccountId { get; set; }

    public required string TokenHash { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset ExpiresAtUtc { get; set; }

    public AccountEntity? Account { get; set; }
}
