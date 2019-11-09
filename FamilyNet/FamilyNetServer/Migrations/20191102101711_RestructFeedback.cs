using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class RestructFeedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "Feedback",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SenderRole",
                table: "Feedback",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "SenderRole",
                table: "Feedback");
        }
    }
}
