using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class Availabilities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VolunteerID",
                table: "Donations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Availabilities",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VolunteerID = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    FromHour = table.Column<DateTime>(nullable: false),
                    VolunteerHours = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availabilities", x => x.ID);
                });

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

            migrationBuilder.DropTable(
                name: "Availabilities");

            migrationBuilder.DropIndex(
                name: "IX_Donations_VolunteerID",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "VolunteerID",
                table: "Donations");
        }
    }
}
