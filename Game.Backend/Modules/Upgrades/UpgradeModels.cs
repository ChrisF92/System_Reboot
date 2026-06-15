using Game.Backend.Modules.Players;
using Game.Backend.Modules.Training;

namespace Game.Backend.Modules.Upgrades;

public sealed record UpgradeState(
    Guid PlayerId,
    ResourceWallet Resources,
    IReadOnlyDictionary<string, int> UpgradeLevels);

public sealed record UpgradeCost(long Matter, long Energy, long Data)
{
    public bool CanPay(ResourceWallet wallet)
    {
        return wallet.Matter >= Matter &&
            wallet.Energy >= Energy &&
            wallet.Data >= Data;
    }

    public TrainingCostResponse ToResponse()
    {
        return new TrainingCostResponse(Matter, Energy, Data);
    }
}
