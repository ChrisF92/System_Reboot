using System.Collections.Concurrent;

namespace Game.Backend.Modules.Players;

public sealed class PlayerRepository
{
    private readonly ConcurrentDictionary<Guid, Player> _players = new();

    public Task<Player> AddAsync(Player player, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _players[player.PlayerId] = player;
        return Task.FromResult(player);
    }

    public Task<Player?> GetByIdAsync(Guid playerId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _players.TryGetValue(playerId, out var player);
        return Task.FromResult(player);
    }
}
