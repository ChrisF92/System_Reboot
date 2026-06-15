using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Xp = table.Column<long>(type: "INTEGER", nullable: false),
                    Matter = table.Column<long>(type: "INTEGER", nullable: false),
                    Energy = table.Column<long>(type: "INTEGER", nullable: false),
                    Data = table.Column<long>(type: "INTEGER", nullable: false),
                    CoreFragments = table.Column<long>(type: "INTEGER", nullable: false),
                    QuantumCores = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "CloudSaves",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SaveVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    SaveJson = table.Column<string>(type: "TEXT", maxLength: 131072, nullable: false),
                    Checksum = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    ClientSavedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    SavedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudSaves", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_CloudSaves_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloudSaves");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
