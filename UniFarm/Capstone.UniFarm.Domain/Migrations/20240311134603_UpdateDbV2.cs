using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateDbV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Batch__OrderId__0F624AF8",
                table: "Batch");

            migrationBuilder.DropForeignKey(
                name: "FK__Transfer__OrderI__160F4887",
                table: "Transfer");

            migrationBuilder.DropIndex(
                name: "IX_Transfer_OrderId",
                table: "Transfer");

            migrationBuilder.DropIndex(
                name: "IX_Batch_OrderId",
                table: "Batch");

            migrationBuilder.DropIndex(
                name: "IX_AccountRole_AccountId",
                table: "AccountRole");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2fd22103-f696-4dd2-ab6e-abd12b6bf6ba"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("40f5895a-cb4a-40da-b759-9cb9f8782302"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ac6031a3-2b90-4549-80bd-1e74b4a30c50"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d2a27cd9-9560-4d7e-8d6a-a8e522a8192a"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e37ea463-edd8-4e63-b407-d23c2c4771c0"));

            migrationBuilder.DropColumn(
                name: "ExpectedReceiveDate",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Transfer");

            migrationBuilder.DropColumn(
                name: "ShipByStationId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Batch");

            migrationBuilder.RenameColumn(
                name: "ExpiredDate",
                table: "Transfer",
                newName: "ReceivedDate");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ProductItemInMenu",
                newName: "Sold");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "Transfer",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Transfer",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OriginPrice",
                table: "ProductItemInMenu",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SalePrice",
                table: "ProductItemInMenu",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerStatus",
                table: "Order",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShipByStationStaffId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TransferId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "AccountRole",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_BatchId",
                table: "Order",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_TransferId",
                table: "Order",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRole_AccountId",
                table: "AccountRole",
                column: "AccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Batch_BatchId",
                table: "Order",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Transfer_TransferId",
                table: "Order",
                column: "TransferId",
                principalTable: "Transfer",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Batch_BatchId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Transfer_TransferId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_BatchId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_TransferId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_AccountRole_AccountId",
                table: "AccountRole");

            migrationBuilder.DropColumn(
                name: "OriginPrice",
                table: "ProductItemInMenu");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "ProductItemInMenu");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShipByStationStaffId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TransferId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "ReceivedDate",
                table: "Transfer",
                newName: "ExpiredDate");

            migrationBuilder.RenameColumn(
                name: "Sold",
                table: "ProductItemInMenu",
                newName: "Price");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Transfer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Transfer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedReceiveDate",
                table: "Transfer",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Transfer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "CustomerStatus",
                table: "Order",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShipByStationId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Batch",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "AccountRole",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("2fd22103-f696-4dd2-ab6e-abd12b6bf6ba"), "7268d486-e833-4e3c-8e19-25ac38f27349", "StationStaff", "STATIONSTAFF" },
                    { new Guid("40f5895a-cb4a-40da-b759-9cb9f8782302"), "1b12817f-4883-499d-b4d4-2ef027b96966", "FarmHub", "FARMHUB" },
                    { new Guid("ac6031a3-2b90-4549-80bd-1e74b4a30c50"), "9d85da47-5723-41e7-9cb2-3eb47b33e4ce", "Admin", "ADMIN" },
                    { new Guid("d2a27cd9-9560-4d7e-8d6a-a8e522a8192a"), "2a6b785b-2c7e-4ca3-931c-a5359377f874", "Customer", "CUSTOMER" },
                    { new Guid("e37ea463-edd8-4e63-b407-d23c2c4771c0"), "711636ed-1bf3-4d80-abc9-ec424237ad43", "CollectedStaff", "COLLECTEDSTAFF" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_OrderId",
                table: "Transfer",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Batch_OrderId",
                table: "Batch",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRole_AccountId",
                table: "AccountRole",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK__Batch__OrderId__0F624AF8",
                table: "Batch",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Transfer__OrderI__160F4887",
                table: "Transfer",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }
    }
}
