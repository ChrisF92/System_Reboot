namespace Game.Backend.Database;

public sealed class AccountEntity
{
    public Guid AccountId { get; set; }

    public required string Email { get; set; }

    public required string NormalizedEmail { get; set; }

    public required string PasswordHash { get; set; }

    public required string PasswordSalt { get; set; }

    public int PasswordIterations { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}
