using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateAccountId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    { new Guid("2e30fa81-4d85-452e-8bb3-b567ab9fecc4"), "30dde507-8886-4b65-939c-5302375dcfd2", "StationStaff", "STATIONSTAFF" },
                    { new Guid("63fc152d-ca30-448b-ade8-bbd0ec813087"), "6dcfdb0b-a061-43aa-8ef4-f0194e30d840", "Customer", "CUSTOMER" },
                    { new Guid("6fb2d08a-0cff-4825-a34e-5fe81efdc6a1"), "b8d07367-22f7-49f7-8b95-9dae72a0b617", "Admin", "ADMIN" },
                    { new Guid("c5036929-ef63-4969-9d6f-5d6eaa607c9b"), "3c7cce1f-d207-4077-bbc3-8a229919b908", "FarmHub", "FARMHUB" },
                    { new Guid("d938fc6d-4195-40dd-bbd6-5c9eddcc5506"), "4e8af29d-abc1-457d-916b-8899e6e29900", "CollectedStaff", "COLLECTEDSTAFF" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
