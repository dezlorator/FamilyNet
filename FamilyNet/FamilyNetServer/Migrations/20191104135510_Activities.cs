using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class Activities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Awards",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Activities",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Awards");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Activities");
        }
    }
}
