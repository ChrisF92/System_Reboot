using Game.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.CloudSaves;

public sealed class CloudSaveRepository
{
    private readonly GameDbContext _dbContext;

    public CloudSaveRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CloudSave?> GetLatestAsync(Guid playerId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.CloudSaves
            .AsNoTracking()
            .SingleOrDefaultAsync(cloudSave => cloudSave.PlayerId == playerId, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<CloudSave> UpsertAsync(CloudSave save, CancellationToken cancellationToken)
    {
        var existingEntity = await _dbContext.CloudSaves
            .SingleOrDefaultAsync(cloudSave => cloudSave.PlayerId == save.PlayerId, cancellationToken);

        if (existingEntity is null)
        {
            _dbContext.CloudSaves.Add(ToEntity(save));
        }
        else
        {
            existingEntity.SaveVersion = save.SaveVersion;
            existingEntity.SaveJson = save.SaveJson;
            existingEntity.Checksum = save.Checksum;
            existingEntity.ClientSavedAtUtc = save.ClientSavedAtUtc;
            existingEntity.SavedAtUtc = save.SavedAtUtc;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return save;
    }

    private static CloudSaveEntity ToEntity(CloudSave save)
    {
        return new CloudSaveEntity
        {
            PlayerId = save.PlayerId,
            SaveVersion = save.SaveVersion,
            SaveJson = save.SaveJson,
            Checksum = save.Checksum,
            ClientSavedAtUtc = save.ClientSavedAtUtc,
            SavedAtUtc = save.SavedAtUtc
        };
    }

    private static CloudSave ToDomain(CloudSaveEntity entity)
    {
        return new CloudSave(
            entity.PlayerId,
            entity.SaveVersion,
            entity.SaveJson,
            entity.Checksum,
            entity.ClientSavedAtUtc,
            entity.SavedAtUtc);
    }
}
