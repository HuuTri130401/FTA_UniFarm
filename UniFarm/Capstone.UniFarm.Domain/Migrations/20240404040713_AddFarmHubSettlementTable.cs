using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class AddFarmHubSettlementTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FarmHubSettlement",
                columns: table => new
                {
                    SettlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FarmHubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalSales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountToBePaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionDetails = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmHubSettlement", x => x.SettlementId);
                    table.ForeignKey(
                        name: "FK_FarmHubSettlement_BusinessDay_BusinessDayId",
                        column: x => x.BusinessDayId,
                        principalTable: "BusinessDay",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FarmHubSettlement_FarmHub_FarmHubId",
                        column: x => x.FarmHubId,
                        principalTable: "FarmHub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmHubSettlement_BusinessDayId",
                table: "FarmHubSettlement",
                column: "BusinessDayId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmHubSettlement_FarmHubId",
                table: "FarmHubSettlement",
                column: "FarmHubId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmHubSettlement");
        }
    }
}
