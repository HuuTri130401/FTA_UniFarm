using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class AddNewColumnToMenuAndImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11dd27b4-f56c-4b44-8163-2f1da2ea0181"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2cbccfd9-84d4-4826-8e79-f9e310fb28c5"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c321e808-0bae-4312-85ad-7104ce0ef837"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("db82caac-692d-4c3b-b3c3-06fc101a4071"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e271d563-8aea-4354-acad-461bc2b15d66"));

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "ProductImage",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "ProductImage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ProductImage",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Menu",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Caption",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Menu");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ProductImage",
                newName: "Image");
        }
    }
}
