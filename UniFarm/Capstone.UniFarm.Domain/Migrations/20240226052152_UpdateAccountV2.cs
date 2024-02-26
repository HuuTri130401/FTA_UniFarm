using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateAccountV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2e30fa81-4d85-452e-8bb3-b567ab9fecc4"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("63fc152d-ca30-448b-ade8-bbd0ec813087"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6fb2d08a-0cff-4825-a34e-5fe81efdc6a1"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5036929-ef63-4969-9d6f-5d6eaa607c9b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d938fc6d-4195-40dd-bbd6-5c9eddcc5506"));

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Account",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Account",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("2a4a2199-45c6-4cef-b81b-7749b7e3a805"), "48f163a5-1acd-4344-b45a-a0894d9b5060", "FarmHub", "FARMHUB" },
                    { new Guid("6e444c8b-7f6c-4956-880a-863e7b61135e"), "3058f854-0f0b-4d4b-8a30-d55fa23c71b4", "CollectedStaff", "COLLECTEDSTAFF" },
                    { new Guid("c6a4f4a3-ec0c-4a44-95f4-fb52abb0de7f"), "6dd34e5d-886f-4063-8335-d60a90d260fa", "Admin", "ADMIN" },
                    { new Guid("dbc7cfbc-9caf-46cd-a0ad-fe1c5df67304"), "f24b3ac9-4f59-4550-8929-d77953e5517c", "Customer", "CUSTOMER" },
                    { new Guid("f1d4b102-d7f8-4b71-8e3a-06d5829847aa"), "090cd6b0-0604-4cae-be47-142e20159b3c", "StationStaff", "STATIONSTAFF" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2a4a2199-45c6-4cef-b81b-7749b7e3a805"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6e444c8b-7f6c-4956-880a-863e7b61135e"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c6a4f4a3-ec0c-4a44-95f4-fb52abb0de7f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("dbc7cfbc-9caf-46cd-a0ad-fe1c5df67304"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f1d4b102-d7f8-4b71-8e3a-06d5829847aa"));

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Account",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
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
                    { new Guid("2e30fa81-4d85-452e-8bb3-b567ab9fecc4"), "30dde507-8886-4b65-939c-5302375dcfd2", "StationStaff", "STATIONSTAFF" },
                    { new Guid("63fc152d-ca30-448b-ade8-bbd0ec813087"), "6dcfdb0b-a061-43aa-8ef4-f0194e30d840", "Customer", "CUSTOMER" },
                    { new Guid("6fb2d08a-0cff-4825-a34e-5fe81efdc6a1"), "b8d07367-22f7-49f7-8b95-9dae72a0b617", "Admin", "ADMIN" },
                    { new Guid("c5036929-ef63-4969-9d6f-5d6eaa607c9b"), "3c7cce1f-d207-4077-bbc3-8a229919b908", "FarmHub", "FARMHUB" },
                    { new Guid("d938fc6d-4195-40dd-bbd6-5c9eddcc5506"), "4e8af29d-abc1-457d-916b-8899e6e29900", "CollectedStaff", "COLLECTEDSTAFF" }
                });
        }
    }
}
