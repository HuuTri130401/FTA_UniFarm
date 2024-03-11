using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateProductItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4824d18a-9760-4bdb-a616-f886476afc3f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("744df328-f61f-408e-bb68-4f111afd6f4b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("95e6eb34-2b81-48c7-a4d6-e2ba2ead8156"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d9e52ec6-b073-40ed-856b-17abd0fc63f2"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("fc2d4c8e-7a9f-45c6-899e-f572f1657a6d"));

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "Station",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProductItemInMenu",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "ProductItemInMenu",
                type: "float",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OutOfStock",
                table: "ProductItem",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sold",
                table: "ProductItem",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Product",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Account",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            //migrationBuilder.InsertData(
            //    table: "Roles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { new Guid("2fd22103-f696-4dd2-ab6e-abd12b6bf6ba"), "7268d486-e833-4e3c-8e19-25ac38f27349", "StationStaff", "STATIONSTAFF" },
            //        { new Guid("40f5895a-cb4a-40da-b759-9cb9f8782302"), "1b12817f-4883-499d-b4d4-2ef027b96966", "FarmHub", "FARMHUB" },
            //        { new Guid("ac6031a3-2b90-4549-80bd-1e74b4a30c50"), "9d85da47-5723-41e7-9cb2-3eb47b33e4ce", "Admin", "ADMIN" },
            //        { new Guid("d2a27cd9-9560-4d7e-8d6a-a8e522a8192a"), "2a6b785b-2c7e-4ca3-931c-a5359377f874", "Customer", "CUSTOMER" },
            //        { new Guid("e37ea463-edd8-4e63-b407-d23c2c4771c0"), "711636ed-1bf3-4d80-abc9-ec424237ad43", "CollectedStaff", "COLLECTEDSTAFF" }
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Quantity",
                table: "ProductItemInMenu");

            migrationBuilder.DropColumn(
                name: "Sold",
                table: "ProductItem");

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "Station",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProductItemInMenu",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OutOfStock",
                table: "ProductItem",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Product",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Account",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("4824d18a-9760-4bdb-a616-f886476afc3f"), "7cf22b5c-d12f-4fb6-84c0-1c29c12712fa", "StationStaff", "STATIONSTAFF" },
                    { new Guid("744df328-f61f-408e-bb68-4f111afd6f4b"), "703c7210-b4bb-4f41-a293-bcba258ff974", "Admin", "ADMIN" },
                    { new Guid("95e6eb34-2b81-48c7-a4d6-e2ba2ead8156"), "8eceb1e0-9319-40b8-9be9-28fd53e7bcc5", "Customer", "CUSTOMER" },
                    { new Guid("d9e52ec6-b073-40ed-856b-17abd0fc63f2"), "a5c2483d-1a91-43cc-bbc9-8b75a323805c", "FarmHub", "FARMHUB" },
                    { new Guid("fc2d4c8e-7a9f-45c6-899e-f572f1657a6d"), "e841cef4-805a-414c-a365-25b5e3924cfd", "CollectedStaff", "COLLECTEDSTAFF" }
                });
        }
    }
}
