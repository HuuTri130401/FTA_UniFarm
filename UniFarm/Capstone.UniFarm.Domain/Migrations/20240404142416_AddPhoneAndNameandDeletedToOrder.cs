using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.UniFarm.Domain.Migrations
{
    public partial class AddPhoneAndNameandDeletedToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderDetail",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Order",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Order");
        }
    }
}
