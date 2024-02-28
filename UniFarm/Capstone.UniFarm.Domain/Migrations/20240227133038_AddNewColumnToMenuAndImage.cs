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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("11dd27b4-f56c-4b44-8163-2f1da2ea0181"), "cd833445-f5b2-447b-b1d6-59c718e2e2a6", "CollectedStaff", "COLLECTEDSTAFF" },
                    { new Guid("2cbccfd9-84d4-4826-8e79-f9e310fb28c5"), "80192500-677e-4c5d-883f-e89bcbba49e2", "Admin", "ADMIN" },
                    { new Guid("c321e808-0bae-4312-85ad-7104ce0ef837"), "e6a11b40-e52f-480c-b1d9-b88bd69d5bc4", "FarmHub", "FARMHUB" },
                    { new Guid("db82caac-692d-4c3b-b3c3-06fc101a4071"), "821b918e-c804-4a5e-89cb-e66ab2bcc30b", "StationStaff", "STATIONSTAFF" },
                    { new Guid("e271d563-8aea-4354-acad-461bc2b15d66"), "57940425-6d6e-4413-8040-2a4ddc520724", "Customer", "CUSTOMER" }
                });
        }
    }
}
