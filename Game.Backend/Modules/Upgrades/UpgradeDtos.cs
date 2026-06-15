using Game.Backend.Modules.Players;
using Game.Backend.Modules.Training;

namespace Game.Backend.Modules.Upgrades;

public sealed record PlayerUpgradesResponse(
    Guid PlayerId,
    IReadOnlyList<PlayerUpgradeResponse> Upgrades,
    ResourceWalletResponse Resources);

public sealed record PlayerUpgradeResponse(
    string UpgradeId,
    string DisplayName,
    string Description,
    int Level,
    TrainingCostResponse NextPurchaseCost,
    UpgradeEffectResponse Effect);

public sealed record UpgradeEffectResponse(
    string EffectType,
    decimal TotalValue);

public sealed record PurchaseUpgradeRequest(string? RequestId);

public sealed record PurchaseUpgradeResponse(
    Guid PlayerId,
    PlayerUpgradeResponse Upgrade,
    TrainingCostResponse CostPaid,
    ResourceWalletResponse Resources);
