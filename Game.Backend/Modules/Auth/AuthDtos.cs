namespace Game.Backend.Modules.Auth;

public sealed record RegisterAccountRequest(string? Email, string? Password);

public sealed record LoginRequest(string? Email, string? Password);

public sealed record AuthResponse(
    Guid AccountId,
    string AccessToken,
    DateTimeOffset ExpiresAtUtc);
