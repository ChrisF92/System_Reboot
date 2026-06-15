using Game.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.Auth;

public sealed class AuthRepository
{
    private readonly GameDbContext _dbContext;

    public AuthRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account?> GetAccountByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(account => account.NormalizedEmail == normalizedEmail, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Account> AddAccountAsync(Account account, CancellationToken cancellationToken)
    {
        _dbContext.Accounts.Add(ToEntity(account));
        await _dbContext.SaveChangesAsync(cancellationToken);
        return account;
    }

    public async Task<AccountSession> AddSessionAsync(
        AccountSession session,
        CancellationToken cancellationToken)
    {
        _dbContext.AccountSessions.Add(ToEntity(session));
        await _dbContext.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task<AccountSession?> GetValidSessionByTokenHashAsync(
        string tokenHash,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.AccountSessions
            .AsNoTracking()
            .SingleOrDefaultAsync(
                session => session.TokenHash == tokenHash && session.ExpiresAtUtc > now,
                cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    private static AccountEntity ToEntity(Account account)
    {
        return new AccountEntity
        {
            AccountId = account.AccountId,
            Email = account.Email,
            NormalizedEmail = account.NormalizedEmail,
            PasswordHash = account.PasswordHash,
            PasswordSalt = account.PasswordSalt,
            PasswordIterations = account.PasswordIterations,
            CreatedAtUtc = account.CreatedAtUtc
        };
    }

    private static Account ToDomain(AccountEntity entity)
    {
        return new Account(
            entity.AccountId,
            entity.Email,
            entity.NormalizedEmail,
            entity.PasswordHash,
            entity.PasswordSalt,
            entity.PasswordIterations,
            entity.CreatedAtUtc);
    }

    private static AccountSessionEntity ToEntity(AccountSession session)
    {
        return new AccountSessionEntity
        {
            SessionId = session.SessionId,
            AccountId = session.AccountId,
            TokenHash = session.TokenHash,
            CreatedAtUtc = session.CreatedAtUtc,
            ExpiresAtUtc = session.ExpiresAtUtc
        };
    }

    private static AccountSession ToDomain(AccountSessionEntity entity)
    {
        return new AccountSession(
            entity.SessionId,
            entity.AccountId,
            entity.TokenHash,
            entity.CreatedAtUtc,
            entity.ExpiresAtUtc);
    }
}
