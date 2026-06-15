namespace Game.Backend.Modules.GameConfig;

public sealed record GameConfigResponse(
    string ConfigVersion,
    ResourceGenerationConfig Resources,
    OfflineProgressConfig OfflineProgress,
    TrainingConfig Training,
    UpgradeConfig Upgrades,
    CombatConfig Combat,
    IReadOnlyList<ResourceDefinitionResponse> ResourceDefinitions);

public sealed record ResourceGenerationConfig(
    decimal MatterPerSecond,
    decimal EnergyPerSecond,
    decimal DataPerSecond);

public sealed record OfflineProgressConfig(
    int EarlyOfflineCapSeconds,
    decimal BaseOfflineEfficiency);

public sealed record TrainingConfig(IReadOnlyList<TrainingStatDefinitionResponse> Stats);

public sealed record TrainingStatDefinitionResponse(
    string StatId,
    string DisplayName,
    long MatterCost,
    long EnergyCost,
    long DataCost);

public sealed record UpgradeConfig(IReadOnlyList<UpgradeDefinitionResponse> Items);

public sealed record UpgradeDefinitionResponse(
    string UpgradeId,
    string DisplayName,
    string Description,
    long MatterCost,
    long EnergyCost,
    long DataCost,
    string EffectType,
    decimal EffectValuePerLevel);

public sealed record CombatConfig(
    int EarlyLoadoutSlots,
    int NormalEncounterMinimumSeconds,
    int NormalEncounterMaximumSeconds);

public sealed record ResourceDefinitionResponse(
    string ResourceId,
    string DisplayName,
    string Purpose);
