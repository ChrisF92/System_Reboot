using Game.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.Players;

public sealed class PlayerRepository
{
    private readonly GameDbContext _dbContext;

    public PlayerRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Player> AddAsync(Player player, CancellationToken cancellationToken)
    {
        _dbContext.Players.Add(ToEntity(player));
        await _dbContext.SaveChangesAsync(cancellationToken);
        return player;
    }

    public async Task<Player?> GetByIdAsync(Guid playerId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Players
            .AsNoTracking()
            .SingleOrDefaultAsync(player => player.PlayerId == playerId, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    private static PlayerEntity ToEntity(Player player)
    {
        return new PlayerEntity
        {
            PlayerId = player.PlayerId,
            DisplayName = player.DisplayName,
            Level = player.Level,
            Xp = player.Xp,
            Matter = player.Resources.Matter,
            Energy = player.Resources.Energy,
            Data = player.Resources.Data,
            CoreFragments = player.Resources.CoreFragments,
            QuantumCores = player.Resources.QuantumCores,
            CreatedAtUtc = player.CreatedAtUtc
        };
    }

    private static Player ToDomain(PlayerEntity entity)
    {
        return new Player(
            entity.PlayerId,
            entity.DisplayName,
            entity.Level,
            entity.Xp,
            new ResourceWallet(
                entity.Matter,
                entity.Energy,
                entity.Data,
                entity.CoreFragments,
                entity.QuantumCores),
            entity.CreatedAtUtc);
    }
}
