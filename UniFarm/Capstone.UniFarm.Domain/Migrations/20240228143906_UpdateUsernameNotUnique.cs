using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateUsernameNotUnique : Migration
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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("03c85ee3-7db3-40fd-b667-3c190057defd"), "3da85a08-b0ed-496d-9bb3-417d71c7b2e4", "CollectedStaff", "COLLECTEDSTAFF" },
                    { new Guid("3bde4839-dfd6-4818-a8fb-23302034879b"), "f0e9a0a3-63d6-4d0a-b3ee-06ffc1418a37", "Admin", "ADMIN" },
                    { new Guid("3c1691b5-c8f6-4c02-aeed-e9b7b8e2fb95"), "c5344179-49e2-4274-b3da-e09a06d45419", "StationStaff", "STATIONSTAFF" },
                    { new Guid("c7b9c720-2234-412f-89dc-d58092c46209"), "7bf1b37e-914d-403e-b932-e774698ce405", "FarmHub", "FARMHUB" },
                    { new Guid("d4504d73-a04f-4a68-b0a5-12c15bde679b"), "4f58f024-9b31-41e8-a092-433f8b170e56", "Customer", "CUSTOMER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_UserName",
                table: "Account",
                column: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Account_UserName",
                table: "Account");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("03c85ee3-7db3-40fd-b667-3c190057defd"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("3bde4839-dfd6-4818-a8fb-23302034879b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("3c1691b5-c8f6-4c02-aeed-e9b7b8e2fb95"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c7b9c720-2234-412f-89dc-d58092c46209"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4504d73-a04f-4a68-b0a5-12c15bde679b"));

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
