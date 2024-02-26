using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateRelationshipAccountRoleInAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountRole_AccountId",
                table: "AccountRole");

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
                    { new Guid("1f9af984-88b5-476e-98c0-3e63d9b31e39"), "549b4041-ecb4-4dc3-9600-50c791f9ba19", "CollectedStaff", "COLLECTEDSTAFF" },
                    { new Guid("2f23aae0-a3e2-45df-8d07-02c94e57e832"), "44b01bee-be76-4530-bd07-7ad105f9480f", "FarmHub", "FARMHUB" },
                    { new Guid("39674c17-5453-4773-9d4d-370d760bd8d1"), "cebc4ed0-fc3c-41a6-9b87-d82c9e87ef77", "StationStaff", "STATIONSTAFF" },
                    { new Guid("39ebdfa0-b1e6-4beb-82e2-e179664e823f"), "48bdcbae-629e-4b12-9fc1-85d7f4adfa2f", "Customer", "CUSTOMER" },
                    { new Guid("a92a98c4-1df6-4b68-bcc5-af88bdebc392"), "0e5629da-017f-4fd6-9eb2-e79965f9b51b", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRole_AccountId",
                table: "AccountRole",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountRole_AccountId",
                table: "AccountRole");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1f9af984-88b5-476e-98c0-3e63d9b31e39"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2f23aae0-a3e2-45df-8d07-02c94e57e832"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("39674c17-5453-4773-9d4d-370d760bd8d1"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("39ebdfa0-b1e6-4beb-82e2-e179664e823f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a92a98c4-1df6-4b68-bcc5-af88bdebc392"));

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

            migrationBuilder.CreateIndex(
                name: "IX_AccountRole_AccountId",
                table: "AccountRole",
                column: "AccountId");
        }
    }
}
