using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateTableTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Transacti__Walle__7C4F7684",
                table: "Transaction");

            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "Transaction",
                newName: "PayerWalletId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_WalletId",
                table: "Transaction",
                newName: "IX_Transaction_PayerWalletId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Transaction",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PayeeWalletId",
                table: "Transaction",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            //migrationBuilder.AddColumn<int>(
            //    name: "NumberOfOrdersInBatch",
            //    table: "Batch",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "PriceTable",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PriceTable", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FarmHubSettlement",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        FarmHubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        BusinessDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        PriceTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        TotalSales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        NumOfOrder = table.Column<int>(type: "int", nullable: false),
            //        DeliveryFeeOfOrder = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        CommissionFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        DailyFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        AmountToBePaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        Profit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FarmHubSettlement", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_FarmHubSettlement_BusinessDay_BusinessDayId",
            //            column: x => x.BusinessDayId,
            //            principalTable: "BusinessDay",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_FarmHubSettlement_FarmHub_FarmHubId",
            //            column: x => x.FarmHubId,
            //            principalTable: "FarmHub",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_FarmHubSettlement_PriceTable_PriceTableId",
            //            column: x => x.PriceTableId,
            //            principalTable: "PriceTable",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PriceTableItem",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        PriceTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        FromAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        ToAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        MinFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        MaxFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PriceTableItem", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_PriceTableItem_PriceTable_PriceTableId",
            //            column: x => x.PriceTableId,
            //            principalTable: "PriceTable",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_FarmHubSettlement_BusinessDayId",
            //    table: "FarmHubSettlement",
            //    column: "BusinessDayId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_FarmHubSettlement_FarmHubId",
            //    table: "FarmHubSettlement",
            //    column: "FarmHubId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_FarmHubSettlement_PriceTableId",
            //    table: "FarmHubSettlement",
            //    column: "PriceTableId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PriceTableItem_PriceTableId",
            //    table: "PriceTableItem",
            //    column: "PriceTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Wallet_PayerWalletId",
                table: "Transaction",
                column: "PayerWalletId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Wallet_PayerWalletId",
                table: "Transaction");

            //migrationBuilder.DropTable(
            //    name: "FarmHubSettlement");

            //migrationBuilder.DropTable(
            //    name: "PriceTableItem");

            //migrationBuilder.DropTable(
            //    name: "PriceTable");

            migrationBuilder.DropColumn(
                name: "PayeeWalletId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "NumberOfOrdersInBatch",
                table: "Batch");

            migrationBuilder.RenameColumn(
                name: "PayerWalletId",
                table: "Transaction",
                newName: "WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_PayerWalletId",
                table: "Transaction",
                newName: "IX_Transaction_WalletId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Transaction",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK__Transacti__Walle__7C4F7684",
                table: "Transaction",
                column: "WalletId",
                principalTable: "Wallet",
                principalColumn: "Id");
        }
    }
}
