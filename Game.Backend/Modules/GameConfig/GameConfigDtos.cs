namespace Game.Backend.Modules.GameConfig;

public sealed record GameConfigResponse(
    string ConfigVersion,
    ResourceGenerationConfig Resources,
    OfflineProgressConfig OfflineProgress,
    CombatConfig Combat,
    IReadOnlyList<ResourceDefinitionResponse> ResourceDefinitions);

public sealed record ResourceGenerationConfig(
    decimal MatterPerSecond,
    decimal EnergyPerSecond,
    decimal DataPerSecond);

public sealed record OfflineProgressConfig(
    int EarlyOfflineCapSeconds,
    decimal BaseOfflineEfficiency);

public sealed record CombatConfig(
    int EarlyLoadoutSlots,
    int NormalEncounterMinimumSeconds,
    int NormalEncounterMaximumSeconds);

public sealed record ResourceDefinitionResponse(
    string ResourceId,
    string DisplayName,
    string Purpose);
