using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class fixLotStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "AuctionLot");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AuctionLot",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AuctionLot");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "AuctionLot",
                nullable: false,
                defaultValue: false);
        }
    }
}
