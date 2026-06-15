using Game.Backend.Shared.Errors;

namespace Game.Backend.Modules.Auth;

public sealed class AuthService
{
    private const int MinimumPasswordLength = 8;
    private static readonly TimeSpan SessionDuration = TimeSpan.FromDays(30);

    private readonly AuthRepository _authRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly TokenHasher _tokenHasher;
    private readonly TimeProvider _timeProvider;

    public AuthService(
        AuthRepository authRepository,
        PasswordHasher passwordHasher,
        TokenHasher tokenHasher,
        TimeProvider timeProvider)
    {
        _authRepository = authRepository;
        _passwordHasher = passwordHasher;
        _tokenHasher = tokenHasher;
        _timeProvider = timeProvider;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterAccountRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ValidationException("missing_register_request", "Registration request is required.");
        }

        var email = ValidateEmail(request.Email);
        var normalizedEmail = NormalizeEmail(email);
        var password = ValidatePassword(request.Password);
        var existingAccount = await _authRepository.GetAccountByNormalizedEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (existingAccount is not null)
        {
            throw new ConflictException("account_already_exists", "An account already exists for this email.");
        }

        var passwordHash = _passwordHasher.HashPassword(password);
        var account = new Account(
            Guid.NewGuid(),
            email,
            normalizedEmail,
            passwordHash.PasswordHash,
            passwordHash.PasswordSalt,
            passwordHash.PasswordIterations,
            _timeProvider.GetUtcNow());

        await _authRepository.AddAccountAsync(account, cancellationToken);
        return await CreateSessionResponseAsync(account.AccountId, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ValidationException("missing_login_request", "Login request is required.");
        }

        var email = ValidateEmail(request.Email);
        var password = ValidatePassword(request.Password);
        var normalizedEmail = NormalizeEmail(email);
        var account = await _authRepository.GetAccountByNormalizedEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (account is null ||
            !_passwordHasher.VerifyPassword(
                password,
                account.PasswordHash,
                account.PasswordSalt,
                account.PasswordIterations))
        {
            throw new ValidationException("invalid_credentials", "Email or password is incorrect.");
        }

        return await CreateSessionResponseAsync(account.AccountId, cancellationToken);
    }

    public async Task<Guid?> GetAccountIdForTokenAsync(
        string accessToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var tokenHash = _tokenHasher.HashToken(accessToken);
        var session = await _authRepository.GetValidSessionByTokenHashAsync(
            tokenHash,
            _timeProvider.GetUtcNow(),
            cancellationToken);

        return session?.AccountId;
    }

    private async Task<AuthResponse> CreateSessionResponseAsync(
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var accessToken = _tokenHasher.CreateToken();
        var now = _timeProvider.GetUtcNow();
        var session = new AccountSession(
            Guid.NewGuid(),
            accountId,
            _tokenHasher.HashToken(accessToken),
            now,
            now.Add(SessionDuration));

        await _authRepository.AddSessionAsync(session, cancellationToken);
        return new AuthResponse(accountId, accessToken, session.ExpiresAtUtc);
    }

    private static string ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ValidationException("email_required", "Email is required.");
        }

        var trimmedEmail = email.Trim();
        if (trimmedEmail.Length > 254 || !trimmedEmail.Contains('@', StringComparison.Ordinal))
        {
            throw new ValidationException("invalid_email", "Email must be a valid email address.");
        }

        return trimmedEmail;
    }

    private static string ValidatePassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < MinimumPasswordLength)
        {
            throw new ValidationException(
                "password_too_short",
                $"Password must be at least {MinimumPasswordLength} characters.");
        }

        return password;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }
}
