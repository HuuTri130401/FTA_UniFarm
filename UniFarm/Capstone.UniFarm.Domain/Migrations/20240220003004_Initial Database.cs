using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("15b9ad57-605e-4086-a05c-0dc50bffb360"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("17604629-b4bb-44dc-ae3f-cfcbe0fd9b66"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("44a92e92-e7e1-4112-82e3-973338c77c1a"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("80dee048-635d-4714-80fe-6230e8ac44de"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c100d3d3-066d-4e6b-9b3e-bd8c44e5d773"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("06bbaab4-f8a4-4c81-892c-3a0a9f61fd47"), "fdfbbe8d-b09b-4771-b0a4-869b057b44b1", "FarmHub", "FARMHUB" },
                    { new Guid("31d0ef4a-12c1-4a20-b6c9-d5666cc350b0"), "434672cb-dd1c-4df8-993d-7c7f7cacbcc7", "CollectedStaff", "COLLECTEDSTAFF" },
                    { new Guid("73e6acd0-6493-4e64-9e19-732e2e87cc18"), "37ff2b97-9803-4262-9d60-d6740586e920", "Customer", "CUSTOMER" },
                    { new Guid("ec241458-7296-4c99-ac42-1088943b2fd1"), "f42a38f7-a296-43ae-86ec-9f9c2a2fb169", "Admin", "ADMIN" },
                    { new Guid("efcdba91-199b-46ac-991a-af30f06c0d39"), "1d89fded-b7cc-4c45-8232-c351ad25971d", "StationStaff", "STATIONSTAFF" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("06bbaab4-f8a4-4c81-892c-3a0a9f61fd47"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("31d0ef4a-12c1-4a20-b6c9-d5666cc350b0"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("73e6acd0-6493-4e64-9e19-732e2e87cc18"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ec241458-7296-4c99-ac42-1088943b2fd1"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("efcdba91-199b-46ac-991a-af30f06c0d39"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("15b9ad57-605e-4086-a05c-0dc50bffb360"), "28769cb2-eb75-4a04-ba26-bf9f7be368be", "Admin", "ADMIN" },
                    { new Guid("17604629-b4bb-44dc-ae3f-cfcbe0fd9b66"), "ecd44b16-4381-4073-a5a3-48a5153063f3", "FarmHub", "FARMHUB" },
                    { new Guid("44a92e92-e7e1-4112-82e3-973338c77c1a"), "3e0a59e5-e87e-4636-ba68-ed5bb5edc41c", "CollectedStaff", "COLLECTEDSTAFF" },
                    { new Guid("80dee048-635d-4714-80fe-6230e8ac44de"), "a10227f2-a80f-4efd-928e-ec9817bf7d56", "Customer", "CUSTOMER" },
                    { new Guid("c100d3d3-066d-4e6b-9b3e-bd8c44e5d773"), "77998ef0-5c87-43d6-8404-973e5bc529e0", "StationStaff", "STATIONSTAFF" }
                });
        }
    }
}
