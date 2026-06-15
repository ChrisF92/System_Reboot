namespace Game.Backend.Modules.GameConfig;

public static class GameConfigDefaults
{
    public static readonly GameConfigResponse InitialConfig = new(
        ConfigVersion: "system-reboot-config-v1",
        Resources: new ResourceGenerationConfig(
            MatterPerSecond: 1.0m,
            EnergyPerSecond: 0.6m,
            DataPerSecond: 0.35m),
        OfflineProgress: new OfflineProgressConfig(
            EarlyOfflineCapSeconds: 7_200,
            BaseOfflineEfficiency: 0.50m),
        Combat: new CombatConfig(
            EarlyLoadoutSlots: 3,
            NormalEncounterMinimumSeconds: 10,
            NormalEncounterMaximumSeconds: 30),
        ResourceDefinitions:
        [
            new ResourceDefinitionResponse(
                "matter",
                "Matter",
                "Basic construction material for upgrades, facilities, and physical expansion."),
            new ResourceDefinitionResponse(
                "energy",
                "Energy",
                "Powers training, avatar systems, drones, facilities, and combat preparation."),
            new ResourceDefinitionResponse(
                "data",
                "Data",
                "Research currency used for technologies, automation, systems, and long-term upgrades."),
            new ResourceDefinitionResponse(
                "core_fragments",
                "Core Fragments",
                "Prestige currency earned from System Reboot and major challenge completions."),
            new ResourceDefinitionResponse(
                "quantum_cores",
                "Quantum Cores",
                "Premium currency reserved for non-pay-to-win purchases.")
        ]);
}
