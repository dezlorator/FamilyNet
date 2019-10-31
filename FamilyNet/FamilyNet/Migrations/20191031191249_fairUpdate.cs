using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class fairUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Paid = table.Column<float>(nullable: false),
                    AuctionLotId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Purchases_AuctionLot_AuctionLotId",
                        column: x => x.AuctionLotId,
                        principalTable: "AuctionLot",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_AuctionLotId",
                table: "Purchases",
                column: "AuctionLotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases");
        }
    }
}
