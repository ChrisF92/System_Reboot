using Game.Backend.Database;
using Game.Backend.Modules.Players;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Modules.Training;

public sealed class TrainingRepository
{
    private readonly GameDbContext _dbContext;

    public TrainingRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TrainingState?> GetAsync(Guid playerId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Players
            .AsNoTracking()
            .SingleOrDefaultAsync(player => player.PlayerId == playerId, cancellationToken);

        return entity is null ? null : ToState(entity);
    }

    public async Task<TrainingState?> UpgradeAsync(
        Guid playerId,
        string statId,
        TrainingCost cost,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Players
            .SingleOrDefaultAsync(player => player.PlayerId == playerId, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        entity.Matter -= cost.Matter;
        entity.Energy -= cost.Energy;
        entity.Data -= cost.Data;

        ApplyStatUpgrade(entity, statId);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToState(entity);
    }

    private static void ApplyStatUpgrade(PlayerEntity entity, string statId)
    {
        switch (statId)
        {
            case TrainingStatIds.Processing:
                entity.Processing += 1;
                break;
            case TrainingStatIds.Integrity:
                entity.Integrity += 1;
                break;
            case TrainingStatIds.Output:
                entity.Output += 1;
                break;
            case TrainingStatIds.Hardening:
                entity.Hardening += 1;
                break;
            case TrainingStatIds.Efficiency:
                entity.Efficiency += 1;
                break;
            case TrainingStatIds.Bandwidth:
                entity.Bandwidth += 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statId), statId, "Unknown training stat.");
        }
    }

    private static TrainingState ToState(PlayerEntity entity)
    {
        return new TrainingState(
            entity.PlayerId,
            new ResourceWallet(
                entity.Matter,
                entity.Energy,
                entity.Data,
                entity.CoreFragments,
                entity.QuantumCores),
            new TrainingStats(
                entity.Processing,
                entity.Integrity,
                entity.Output,
                entity.Hardening,
                entity.Efficiency,
                entity.Bandwidth));
    }
}

public static class TrainingStatIds
{
    public const string Processing = "processing";
    public const string Integrity = "integrity";
    public const string Output = "output";
    public const string Hardening = "hardening";
    public const string Efficiency = "efficiency";
    public const string Bandwidth = "bandwidth";
}
