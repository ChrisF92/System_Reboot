using Game.Backend.Database;
using Game.Backend.Modules.Players;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.Resources;

public sealed class ResourceRepository
{
    private readonly GameDbContext _dbContext;

    public ResourceRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PlayerResourceState?> GetAsync(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Players
            .AsNoTracking()
            .SingleOrDefaultAsync(player => player.PlayerId == playerId, cancellationToken);

        return entity is null ? null : ToResourceState(entity);
    }

    public async Task<PlayerResourceState?> AddOfflineGainsAsync(
        Guid playerId,
        ResourceGain gain,
        DateTimeOffset claimedAtUtc,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Players
            .SingleOrDefaultAsync(player => player.PlayerId == playerId, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        entity.Matter += gain.Matter;
        entity.Energy += gain.Energy;
        entity.Data += gain.Data;
        entity.LastResourceClaimedAtUtc = claimedAtUtc;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToResourceState(entity);
    }

    private static PlayerResourceState ToResourceState(PlayerEntity entity)
    {
        return new PlayerResourceState(
            entity.PlayerId,
            new ResourceWallet(
                entity.Matter,
                entity.Energy,
                entity.Data,
                entity.CoreFragments,
                entity.QuantumCores),
            entity.LastResourceClaimedAtUtc);
    }
}

public sealed record ResourceGain(long Matter, long Energy, long Data)
{
    public bool HasAnyGain => Matter > 0 || Energy > 0 || Data > 0;
}
