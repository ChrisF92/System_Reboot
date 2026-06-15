using Game.Backend.Modules.Players;

namespace Game.Backend.Modules.Training;

public sealed record TrainingState(
    Guid PlayerId,
    ResourceWallet Resources,
    TrainingStats Stats);

public sealed record TrainingCost(long Matter, long Energy, long Data)
{
    public bool CanPay(ResourceWallet wallet)
    {
        return wallet.Matter >= Matter &&
            wallet.Energy >= Energy &&
            wallet.Data >= Data;
    }
}
