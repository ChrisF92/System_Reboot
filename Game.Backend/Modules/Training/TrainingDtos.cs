using Game.Backend.Modules.Players;

namespace Game.Backend.Modules.Training;

public sealed record TrainingResponse(
    Guid PlayerId,
    IReadOnlyList<TrainingStatResponse> Stats,
    ResourceWalletResponse Resources);

public sealed record TrainingStatResponse(
    string StatId,
    string DisplayName,
    int Level,
    TrainingCostResponse NextUpgradeCost);

public sealed record TrainingCostResponse(
    long Matter,
    long Energy,
    long Data);

public sealed record UpgradeTrainingRequest(string? RequestId);

public sealed record UpgradeTrainingResponse(
    Guid PlayerId,
    TrainingStatResponse UpgradedStat,
    TrainingCostResponse CostPaid,
    ResourceWalletResponse Resources);
