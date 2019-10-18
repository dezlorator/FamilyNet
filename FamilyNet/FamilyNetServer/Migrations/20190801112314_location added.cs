using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class locationadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Donations_DonationItemID",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "MapCoordX",
                table: "Orphanages");

            migrationBuilder.DropColumn(
                name: "MapCoordY",
                table: "Orphanages");

            migrationBuilder.AddColumn<int>(
                name: "LocationID",
                table: "Orphanages",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MapCoordX = table.Column<float>(nullable: true),
                    MapCoordY = table.Column<float>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orphanages_LocationID",
                table: "Orphanages",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DonationItemID",
                table: "Donations",
                column: "DonationItemID",
                unique: true,
                filter: "[DonationItemID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Orphanages_Location_LocationID",
                table: "Orphanages",
                column: "LocationID",
                principalTable: "Location",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orphanages_Location_LocationID",
                table: "Orphanages");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Orphanages_LocationID",
                table: "Orphanages");

            migrationBuilder.DropIndex(
                name: "IX_Donations_DonationItemID",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "LocationID",
                table: "Orphanages");

            migrationBuilder.AddColumn<float>(
                name: "MapCoordX",
                table: "Orphanages",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "MapCoordY",
                table: "Orphanages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DonationItemID",
                table: "Donations",
                column: "DonationItemID");
        }
    }
}
