using Game.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.GameConfig;

public sealed class GameConfigRepository
{
    private readonly GameDbContext _dbContext;

    public GameConfigRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GameConfigVersion?> GetActiveAsync(CancellationToken cancellationToken)
    {
        var entity = await _dbContext.GameConfigVersions
            .AsNoTracking()
            .SingleOrDefaultAsync(config => config.IsActive, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<GameConfigVersion?> GetByVersionAsync(
        string configVersion,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.GameConfigVersions
            .AsNoTracking()
            .SingleOrDefaultAsync(config => config.ConfigVersion == configVersion, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task ActivateAsync(
        string configVersion,
        DateTimeOffset activatedAtUtc,
        CancellationToken cancellationToken)
    {
        var configs = await _dbContext.GameConfigVersions
            .ToListAsync(cancellationToken);

        foreach (var config in configs)
        {
            config.IsActive = string.Equals(config.ConfigVersion, configVersion, StringComparison.Ordinal);
            config.ActivatedAtUtc = config.IsActive ? activatedAtUtc : null;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static GameConfigVersion ToDomain(GameConfigVersionEntity entity)
    {
        return new GameConfigVersion(
            entity.ConfigVersion,
            entity.ConfigJson,
            entity.IsActive,
            entity.CreatedAtUtc,
            entity.ActivatedAtUtc);
    }
}
