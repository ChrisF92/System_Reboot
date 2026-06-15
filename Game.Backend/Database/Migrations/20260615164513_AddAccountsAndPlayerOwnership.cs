using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountsAndPlayerOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    PasswordSalt = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    PasswordIterations = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns:
                [
                    "AccountId",
                    "Email",
                    "NormalizedEmail",
                    "PasswordHash",
                    "PasswordSalt",
                    "PasswordIterations",
                    "CreatedAtUtc"
                ],
                values:
                [
                    new Guid("00000000-0000-0000-0000-000000000000"),
                    "legacy-local@example.invalid",
                    "LEGACY-LOCAL@EXAMPLE.INVALID",
                    "legacy",
                    "legacy",
                    1,
                    new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero)
                ]);

            migrationBuilder.CreateTable(
                name: "AccountSessions",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_AccountSessions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_AccountId",
                table: "Players",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_NormalizedEmail",
                table: "Accounts",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountSessions_AccountId",
                table: "AccountSessions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSessions_TokenHash",
                table: "AccountSessions",
                column: "TokenHash",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Accounts_AccountId",
                table: "Players",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Accounts_AccountId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "AccountSessions");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Players_AccountId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Players");
        }
    }
}
