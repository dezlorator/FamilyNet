using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class addeddonationtoorphanages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrphanageID",
                table: "DonationItem",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonationItem_OrphanageID",
                table: "DonationItem",
                column: "OrphanageID");

            migrationBuilder.AddForeignKey(
                name: "FK_DonationItem_Orphanages_OrphanageID",
                table: "DonationItem",
                column: "OrphanageID",
                principalTable: "Orphanages",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonationItem_Orphanages_OrphanageID",
                table: "DonationItem");

            migrationBuilder.DropIndex(
                name: "IX_DonationItem_OrphanageID",
                table: "DonationItem");

            migrationBuilder.DropColumn(
                name: "OrphanageID",
                table: "DonationItem");
        }
    }
}
