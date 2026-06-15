using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchasesAndEntitlements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseReceipts",
                columns: table => new
                {
                    PurchaseReceiptId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Store = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    ProductId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    TransactionId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ReceiptHash = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ValidatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReceipts", x => x.PurchaseReceiptId);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipts_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entitlements",
                columns: table => new
                {
                    EntitlementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    SourcePurchaseReceiptId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GrantedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entitlements", x => x.EntitlementId);
                    table.ForeignKey(
                        name: "FK_Entitlements_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entitlements_PurchaseReceipts_SourcePurchaseReceiptId",
                        column: x => x.SourcePurchaseReceiptId,
                        principalTable: "PurchaseReceipts",
                        principalColumn: "PurchaseReceiptId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entitlements_AccountId_ProductId",
                table: "Entitlements",
                columns: new[] { "AccountId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entitlements_SourcePurchaseReceiptId",
                table: "Entitlements",
                column: "SourcePurchaseReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipts_AccountId",
                table: "PurchaseReceipts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipts_PlayerId",
                table: "PurchaseReceipts",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipts_ReceiptHash",
                table: "PurchaseReceipts",
                column: "ReceiptHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entitlements");

            migrationBuilder.DropTable(
                name: "PurchaseReceipts");
        }
    }
}
