using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateBatchField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectedHubAddress",
                table: "CollectedHub");

            migrationBuilder.AddColumn<Guid>(
                name: "CollectedStaffProcessId",
                table: "Batch",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectedStaffProcessId",
                table: "Batch");

            migrationBuilder.AddColumn<string>(
                name: "CollectedHubAddress",
                table: "CollectedHub",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
