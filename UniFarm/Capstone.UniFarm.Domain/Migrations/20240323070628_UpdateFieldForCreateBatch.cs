using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateFieldForCreateBatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShipDate",
                table: "Batch",
                newName: "FarmShipDate");

            migrationBuilder.RenameColumn(
                name: "ReceiveDate",
                table: "Batch",
                newName: "CollectedHubReceiveDate");

            migrationBuilder.AddColumn<string>(
                name: "CollectedHubAddress",
                table: "CollectedHub",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OpenDay",
                table: "BusinessDay",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeedBackImage",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedDescription",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectedHubAddress",
                table: "CollectedHub");

            migrationBuilder.DropColumn(
                name: "FeedBackImage",
                table: "Batch");

            migrationBuilder.DropColumn(
                name: "ReceivedDescription",
                table: "Batch");

            migrationBuilder.RenameColumn(
                name: "FarmShipDate",
                table: "Batch",
                newName: "ShipDate");

            migrationBuilder.RenameColumn(
                name: "CollectedHubReceiveDate",
                table: "Batch",
                newName: "ReceiveDate");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OpenDay",
                table: "BusinessDay",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
