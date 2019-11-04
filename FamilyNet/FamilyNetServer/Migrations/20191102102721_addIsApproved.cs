using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class addIsApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "AuctionLot",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "AuctionLot");
        }
    }
}
