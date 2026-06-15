using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerUpgrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerUpgrades",
                columns: table => new
                {
                    PlayerUpgradeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UpgradeId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUpgrades", x => x.PlayerUpgradeId);
                    table.ForeignKey(
                        name: "FK_PlayerUpgrades_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUpgrades_PlayerId_UpgradeId",
                table: "PlayerUpgrades",
                columns: new[] { "PlayerId", "UpgradeId" },
                unique: true);

            migrationBuilder.UpdateData(
                table: "GameConfigVersions",
                keyColumn: "ConfigVersion",
                keyValue: "system-reboot-config-v1",
                column: "ConfigJson",
                value: "{\"configVersion\":\"system-reboot-config-v1\",\"resources\":{\"matterPerSecond\":1.0,\"energyPerSecond\":0.6,\"dataPerSecond\":0.35},\"offlineProgress\":{\"earlyOfflineCapSeconds\":7200,\"baseOfflineEfficiency\":0.5},\"training\":{\"stats\":[{\"statId\":\"processing\",\"displayName\":\"Processing\",\"matterCost\":10,\"energyCost\":5,\"dataCost\":2},{\"statId\":\"integrity\",\"displayName\":\"Integrity\",\"matterCost\":12,\"energyCost\":4,\"dataCost\":0},{\"statId\":\"output\",\"displayName\":\"Output\",\"matterCost\":8,\"energyCost\":8,\"dataCost\":1},{\"statId\":\"hardening\",\"displayName\":\"Hardening\",\"matterCost\":15,\"energyCost\":3,\"dataCost\":0},{\"statId\":\"efficiency\",\"displayName\":\"Efficiency\",\"matterCost\":8,\"energyCost\":4,\"dataCost\":4},{\"statId\":\"bandwidth\",\"displayName\":\"Bandwidth\",\"matterCost\":5,\"energyCost\":5,\"dataCost\":5}]},\"upgrades\":{\"items\":[{\"upgradeId\":\"core_processing_booster\",\"displayName\":\"Core Processing Booster\",\"description\":\"Improves Processing-focused progression hooks.\",\"matterCost\":25,\"energyCost\":10,\"dataCost\":5,\"effectType\":\"processing_bonus\",\"effectValuePerLevel\":0.05},{\"upgradeId\":\"matter_harvester\",\"displayName\":\"Matter Harvester\",\"description\":\"Improves Matter-focused progression hooks.\",\"matterCost\":30,\"energyCost\":5,\"dataCost\":0,\"effectType\":\"matter_generation_bonus\",\"effectValuePerLevel\":0.1},{\"upgradeId\":\"energy_conduit\",\"displayName\":\"Energy Conduit\",\"description\":\"Improves Energy-focused progression hooks.\",\"matterCost\":15,\"energyCost\":25,\"dataCost\":0,\"effectType\":\"energy_generation_bonus\",\"effectValuePerLevel\":0.1},{\"upgradeId\":\"data_lattice\",\"displayName\":\"Data Lattice\",\"description\":\"Improves Data-focused progression hooks.\",\"matterCost\":10,\"energyCost\":10,\"dataCost\":15,\"effectType\":\"data_generation_bonus\",\"effectValuePerLevel\":0.1}]},\"combat\":{\"earlyLoadoutSlots\":3,\"normalEncounterMinimumSeconds\":10,\"normalEncounterMaximumSeconds\":30},\"resourceDefinitions\":[{\"resourceId\":\"matter\",\"displayName\":\"Matter\",\"purpose\":\"Basic construction material for upgrades, facilities, and physical expansion.\"},{\"resourceId\":\"energy\",\"displayName\":\"Energy\",\"purpose\":\"Powers training, avatar systems, drones, facilities, and combat preparation.\"},{\"resourceId\":\"data\",\"displayName\":\"Data\",\"purpose\":\"Research currency used for technologies, automation, systems, and long-term upgrades.\"},{\"resourceId\":\"core_fragments\",\"displayName\":\"Core Fragments\",\"purpose\":\"Prestige currency earned from System Reboot and major challenge completions.\"},{\"resourceId\":\"quantum_cores\",\"displayName\":\"Quantum Cores\",\"purpose\":\"Premium currency reserved for non-pay-to-win purchases.\"}]}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerUpgrades");
        }
    }
}
