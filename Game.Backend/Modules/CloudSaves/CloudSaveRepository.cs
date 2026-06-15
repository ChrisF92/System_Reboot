using System.Collections.Concurrent;

namespace Game.Backend.Modules.CloudSaves;

public sealed class CloudSaveRepository
{
    private readonly ConcurrentDictionary<Guid, CloudSave> _saves = new();

    public Task<CloudSave?> GetLatestAsync(Guid playerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _saves.TryGetValue(playerId, out var save);
        return Task.FromResult(save);
    }

    public Task<CloudSave> UpsertAsync(CloudSave save, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _saves[save.PlayerId] = save;
        return Task.FromResult(save);
    }
}
