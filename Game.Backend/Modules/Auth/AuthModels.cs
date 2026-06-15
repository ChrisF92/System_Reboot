namespace Game.Backend.Modules.Auth;

public sealed record Account(
    Guid AccountId,
    string Email,
    string NormalizedEmail,
    string PasswordHash,
    string PasswordSalt,
    int PasswordIterations,
    DateTimeOffset CreatedAtUtc);

public sealed record AccountSession(
    Guid SessionId,
    Guid AccountId,
    string TokenHash,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset? RevokedAtUtc);
