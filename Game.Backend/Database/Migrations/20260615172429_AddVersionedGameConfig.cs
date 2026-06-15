using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionedGameConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameConfigVersions",
                columns: table => new
                {
                    ConfigVersion = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ConfigJson = table.Column<string>(type: "TEXT", maxLength: 32768, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ActivatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameConfigVersions", x => x.ConfigVersion);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameConfigVersions_IsActive",
                table: "GameConfigVersions",
                column: "IsActive");

            migrationBuilder.InsertData(
                table: "GameConfigVersions",
                columns:
                [
                    "ConfigVersion",
                    "ConfigJson",
                    "IsActive",
                    "CreatedAtUtc",
                    "ActivatedAtUtc"
                ],
                values:
                [
                    "system-reboot-config-v1",
                    "{\"configVersion\":\"system-reboot-config-v1\",\"resources\":{\"matterPerSecond\":1.0,\"energyPerSecond\":0.6,\"dataPerSecond\":0.35},\"offlineProgress\":{\"earlyOfflineCapSeconds\":7200,\"baseOfflineEfficiency\":0.5},\"combat\":{\"earlyLoadoutSlots\":3,\"normalEncounterMinimumSeconds\":10,\"normalEncounterMaximumSeconds\":30},\"resourceDefinitions\":[{\"resourceId\":\"matter\",\"displayName\":\"Matter\",\"purpose\":\"Basic construction material for upgrades, facilities, and physical expansion.\"},{\"resourceId\":\"energy\",\"displayName\":\"Energy\",\"purpose\":\"Powers training, avatar systems, drones, facilities, and combat preparation.\"},{\"resourceId\":\"data\",\"displayName\":\"Data\",\"purpose\":\"Research currency used for technologies, automation, systems, and long-term upgrades.\"},{\"resourceId\":\"core_fragments\",\"displayName\":\"Core Fragments\",\"purpose\":\"Prestige currency earned from System Reboot and major challenge completions.\"},{\"resourceId\":\"quantum_cores\",\"displayName\":\"Quantum Cores\",\"purpose\":\"Premium currency reserved for non-pay-to-win purchases.\"}]}",
                    true,
                    new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero)
                ]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameConfigVersions");
        }
    }
}
