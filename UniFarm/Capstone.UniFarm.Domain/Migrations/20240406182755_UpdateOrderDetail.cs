using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class UpdateOrderDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginUnitPrice",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "TotalOriginPrice",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "TotalFarmHubPrice",
                table: "Order");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OriginUnitPrice",
                table: "OrderDetail",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOriginPrice",
                table: "OrderDetail",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFarmHubPrice",
                table: "Order",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
