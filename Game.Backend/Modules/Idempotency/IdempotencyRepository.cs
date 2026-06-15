using Game.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.Idempotency;

public sealed class IdempotencyRepository
{
    private readonly GameDbContext _dbContext;

    public IdempotencyRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IdempotencyKey?> GetAsync(
        Guid accountId,
        string action,
        string requestId,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.IdempotencyKeys
            .AsNoTracking()
            .SingleOrDefaultAsync(
                key => key.AccountId == accountId &&
                    key.Action == action &&
                    key.RequestId == requestId,
                cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task AddAsync(IdempotencyKey key, CancellationToken cancellationToken)
    {
        _dbContext.IdempotencyKeys.Add(ToEntity(key));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IdempotencyKey ToDomain(IdempotencyKeyEntity entity)
    {
        return new IdempotencyKey(
            entity.IdempotencyKeyId,
            entity.AccountId,
            entity.PlayerId,
            entity.Action,
            entity.RequestId,
            entity.RequestHash,
            entity.ResponseJson,
            entity.CreatedAtUtc);
    }

    private static IdempotencyKeyEntity ToEntity(IdempotencyKey key)
    {
        return new IdempotencyKeyEntity
        {
            IdempotencyKeyId = key.IdempotencyKeyId,
            AccountId = key.AccountId,
            PlayerId = key.PlayerId,
            Action = key.Action,
            RequestId = key.RequestId,
            RequestHash = key.RequestHash,
            ResponseJson = key.ResponseJson,
            CreatedAtUtc = key.CreatedAtUtc
        };
    }
}
