using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class AddTableManagePrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistributionDistance",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TransactionDetails",
                table: "FarmHubSettlement");

            migrationBuilder.RenameColumn(
                name: "MaintenanceFee",
                table: "FarmHubSettlement",
                newName: "Profit");

            migrationBuilder.RenameColumn(
                name: "FixedFee",
                table: "FarmHubSettlement",
                newName: "DeliveryFeeOfOrder");

            migrationBuilder.AddColumn<decimal>(
                name: "DailyFee",
                table: "FarmHubSettlement",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumOfOrder",
                table: "FarmHubSettlement",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PriceTableId",
                table: "FarmHubSettlement",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PriceTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceTableItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTableItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceTableItem_PriceTable_PriceTableId",
                        column: x => x.PriceTableId,
                        principalTable: "PriceTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmHubSettlement_PriceTableId",
                table: "FarmHubSettlement",
                column: "PriceTableId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceTableItem_PriceTableId",
                table: "PriceTableItem",
                column: "PriceTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmHubSettlement_PriceTable_PriceTableId",
                table: "FarmHubSettlement",
                column: "PriceTableId",
                principalTable: "PriceTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmHubSettlement_PriceTable_PriceTableId",
                table: "FarmHubSettlement");

            migrationBuilder.DropTable(
                name: "PriceTableItem");

            migrationBuilder.DropTable(
                name: "PriceTable");

            migrationBuilder.DropIndex(
                name: "IX_FarmHubSettlement_PriceTableId",
                table: "FarmHubSettlement");

            migrationBuilder.DropColumn(
                name: "DailyFee",
                table: "FarmHubSettlement");

            migrationBuilder.DropColumn(
                name: "NumOfOrder",
                table: "FarmHubSettlement");

            migrationBuilder.DropColumn(
                name: "PriceTableId",
                table: "FarmHubSettlement");

            migrationBuilder.RenameColumn(
                name: "Profit",
                table: "FarmHubSettlement",
                newName: "MaintenanceFee");

            migrationBuilder.RenameColumn(
                name: "DeliveryFeeOfOrder",
                table: "FarmHubSettlement",
                newName: "FixedFee");

            migrationBuilder.AddColumn<double>(
                name: "DistributionDistance",
                table: "Order",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "Order",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionDetails",
                table: "FarmHubSettlement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
