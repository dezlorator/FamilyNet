using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class donationvolunteer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VolunteerID",
                table: "Donations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donations_VolunteerID",
                table: "Donations",
                column: "VolunteerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_Volunteers_VolunteerID",
                table: "Donations",
                column: "VolunteerID",
                principalTable: "Volunteers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donations_Volunteers_VolunteerID",
                table: "Donations");

            migrationBuilder.DropIndex(
                name: "IX_Donations_VolunteerID",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "VolunteerID",
                table: "Donations");
        }
    }
}
