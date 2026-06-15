using Game.Backend.Database;
using Game.Backend.Modules.Players;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.Upgrades;

public sealed class UpgradeRepository
{
    private readonly GameDbContext _dbContext;

    public UpgradeRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UpgradeState?> GetAsync(Guid playerId, CancellationToken cancellationToken)
    {
        var player = await _dbContext.Players
            .AsNoTracking()
            .SingleOrDefaultAsync(player => player.PlayerId == playerId, cancellationToken);

        if (player is null)
        {
            return null;
        }

        var upgradeLevels = await _dbContext.PlayerUpgrades
            .AsNoTracking()
            .Where(upgrade => upgrade.PlayerId == playerId)
            .ToDictionaryAsync(upgrade => upgrade.UpgradeId, upgrade => upgrade.Level, cancellationToken);

        return ToState(player, upgradeLevels);
    }

    public async Task<UpgradeState?> PurchaseAsync(
        Guid playerId,
        string upgradeId,
        UpgradeCost cost,
        CancellationToken cancellationToken)
    {
        var player = await _dbContext.Players
            .SingleOrDefaultAsync(player => player.PlayerId == playerId, cancellationToken);

        if (player is null)
        {
            return null;
        }

        player.Matter -= cost.Matter;
        player.Energy -= cost.Energy;
        player.Data -= cost.Data;

        var upgrade = await _dbContext.PlayerUpgrades
            .SingleOrDefaultAsync(
                playerUpgrade => playerUpgrade.PlayerId == playerId &&
                    playerUpgrade.UpgradeId == upgradeId,
                cancellationToken);

        if (upgrade is null)
        {
            _dbContext.PlayerUpgrades.Add(new PlayerUpgradeEntity
            {
                PlayerUpgradeId = Guid.NewGuid(),
                PlayerId = playerId,
                UpgradeId = upgradeId,
                Level = 1
            });
        }
        else
        {
            upgrade.Level += 1;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var upgradeLevels = await _dbContext.PlayerUpgrades
            .AsNoTracking()
            .Where(playerUpgrade => playerUpgrade.PlayerId == playerId)
            .ToDictionaryAsync(playerUpgrade => playerUpgrade.UpgradeId, playerUpgrade => playerUpgrade.Level, cancellationToken);

        return ToState(player, upgradeLevels);
    }

    private static UpgradeState ToState(
        PlayerEntity player,
        IReadOnlyDictionary<string, int> upgradeLevels)
    {
        return new UpgradeState(
            player.PlayerId,
            new ResourceWallet(
                player.Matter,
                player.Energy,
                player.Data,
                player.CoreFragments,
                player.QuantumCores),
            upgradeLevels);
    }
}
