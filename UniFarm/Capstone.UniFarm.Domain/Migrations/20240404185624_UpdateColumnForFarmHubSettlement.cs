using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateColumnForFarmHubSettlement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherFees",
                table: "FarmHubSettlement",
                newName: "MaintenanceFee");

            migrationBuilder.AddColumn<decimal>(
                name: "FixedFee",
                table: "FarmHubSettlement",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedFee",
                table: "FarmHubSettlement");

            migrationBuilder.RenameColumn(
                name: "MaintenanceFee",
                table: "FarmHubSettlement",
                newName: "OtherFees");
        }
    }
}
