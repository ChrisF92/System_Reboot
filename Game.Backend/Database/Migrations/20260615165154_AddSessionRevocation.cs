using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionRevocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RevokedAtUtc",
                table: "AccountSessions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevokedAtUtc",
                table: "AccountSessions");
        }
    }
}
