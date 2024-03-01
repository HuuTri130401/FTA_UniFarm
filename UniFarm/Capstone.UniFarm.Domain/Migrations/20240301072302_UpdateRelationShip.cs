using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateRelationShip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Product__FarmHub__5812160E",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK__ProductIm__Produ__5CD6CB2B",
                table: "ProductImage");

            migrationBuilder.DropIndex(
                name: "IX_Product_FarmHubId",
                table: "Product");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4f573410-9019-4308-bf2b-5ac991d69697"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("64ffe38a-e193-4f25-9170-ea292588f8e2"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("7b28501d-25bd-4095-bed0-1a5bd55f7ab8"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c7244650-5abd-4e24-99a3-393069d00145"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f562d514-2d12-4527-9f22-26133c651d41"));

            migrationBuilder.DropColumn(
                name: "FarmHubId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SpecialTag",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductImage",
                newName: "ProductItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                newName: "IX_ProductImage_ProductItemId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FarmHubId",
                table: "ProductItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "OutOfStock",
                table: "ProductItem",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductOrigin",
                table: "ProductItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpecialTag",
                table: "ProductItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageType",
                table: "ProductItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ProductItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "ProductImage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                type: "datetime2",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Product",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Menu",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Menu",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.CreateIndex(
                name: "IX_ProductItem_FarmHubId",
                table: "ProductItem",
                column: "FarmHubId");

            migrationBuilder.AddForeignKey(
                name: "FK__ProductIm__Produ__5CD6CB2B",
                table: "ProductImage",
                column: "ProductItemId",
                principalTable: "ProductItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItem_FarmHub_FarmHubId",
                table: "ProductItem",
                column: "FarmHubId",
                principalTable: "FarmHub",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ProductIm__Produ__5CD6CB2B",
                table: "ProductImage");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductItem_FarmHub_FarmHubId",
                table: "ProductItem");

            migrationBuilder.DropIndex(
                name: "IX_ProductItem_FarmHubId",
                table: "ProductItem");

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

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "FarmHubId",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "OutOfStock",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "ProductOrigin",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "SpecialTag",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "StorageType",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "ProductItemId",
                table: "ProductImage",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductImage_ProductItemId",
                table: "ProductImage",
                newName: "IX_ProductImage_ProductId");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "ProductImage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "FarmHubId",
                table: "Product",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialTag",
                table: "Product",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Menu",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Menu",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("4f573410-9019-4308-bf2b-5ac991d69697"), "b3cd8f3b-05f2-4b39-a0ce-fb6f27b52d30", "CollectedStaff", "COLLECTEDSTAFF" },
                    { new Guid("64ffe38a-e193-4f25-9170-ea292588f8e2"), "2ae7ca0d-b5db-46e0-a802-43b25d303695", "FarmHub", "FARMHUB" },
                    { new Guid("7b28501d-25bd-4095-bed0-1a5bd55f7ab8"), "b734938e-84d8-4078-b310-3e5b5f020885", "Customer", "CUSTOMER" },
                    { new Guid("c7244650-5abd-4e24-99a3-393069d00145"), "dd0604e0-56b7-46ce-a39a-7a79d89379df", "StationStaff", "STATIONSTAFF" },
                    { new Guid("f562d514-2d12-4527-9f22-26133c651d41"), "6d73e1f1-8625-4eb6-962f-ea2b4c4056ae", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_FarmHubId",
                table: "Product",
                column: "FarmHubId");

            migrationBuilder.AddForeignKey(
                name: "FK__Product__FarmHub__5812160E",
                table: "Product",
                column: "FarmHubId",
                principalTable: "FarmHub",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__ProductIm__Produ__5CD6CB2B",
                table: "ProductImage",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");
        }
    }
}
