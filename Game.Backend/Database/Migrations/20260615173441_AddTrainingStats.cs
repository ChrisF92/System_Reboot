using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bandwidth",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Efficiency",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Hardening",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Integrity",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Output",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Processing",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "GameConfigVersions",
                keyColumn: "ConfigVersion",
                keyValue: "system-reboot-config-v1",
                column: "ConfigJson",
                value: "{\"configVersion\":\"system-reboot-config-v1\",\"resources\":{\"matterPerSecond\":1.0,\"energyPerSecond\":0.6,\"dataPerSecond\":0.35},\"offlineProgress\":{\"earlyOfflineCapSeconds\":7200,\"baseOfflineEfficiency\":0.5},\"training\":{\"stats\":[{\"statId\":\"processing\",\"displayName\":\"Processing\",\"matterCost\":10,\"energyCost\":5,\"dataCost\":2},{\"statId\":\"integrity\",\"displayName\":\"Integrity\",\"matterCost\":12,\"energyCost\":4,\"dataCost\":0},{\"statId\":\"output\",\"displayName\":\"Output\",\"matterCost\":8,\"energyCost\":8,\"dataCost\":1},{\"statId\":\"hardening\",\"displayName\":\"Hardening\",\"matterCost\":15,\"energyCost\":3,\"dataCost\":0},{\"statId\":\"efficiency\",\"displayName\":\"Efficiency\",\"matterCost\":8,\"energyCost\":4,\"dataCost\":4},{\"statId\":\"bandwidth\",\"displayName\":\"Bandwidth\",\"matterCost\":5,\"energyCost\":5,\"dataCost\":5}]},\"combat\":{\"earlyLoadoutSlots\":3,\"normalEncounterMinimumSeconds\":10,\"normalEncounterMaximumSeconds\":30},\"resourceDefinitions\":[{\"resourceId\":\"matter\",\"displayName\":\"Matter\",\"purpose\":\"Basic construction material for upgrades, facilities, and physical expansion.\"},{\"resourceId\":\"energy\",\"displayName\":\"Energy\",\"purpose\":\"Powers training, avatar systems, drones, facilities, and combat preparation.\"},{\"resourceId\":\"data\",\"displayName\":\"Data\",\"purpose\":\"Research currency used for technologies, automation, systems, and long-term upgrades.\"},{\"resourceId\":\"core_fragments\",\"displayName\":\"Core Fragments\",\"purpose\":\"Prestige currency earned from System Reboot and major challenge completions.\"},{\"resourceId\":\"quantum_cores\",\"displayName\":\"Quantum Cores\",\"purpose\":\"Premium currency reserved for non-pay-to-win purchases.\"}]}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bandwidth",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Efficiency",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Hardening",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Integrity",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Output",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Processing",
                table: "Players");
        }
    }
}
