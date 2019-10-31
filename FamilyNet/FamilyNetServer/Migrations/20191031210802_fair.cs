using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class fair : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "AuctionLot",
                newName: "DateStart");

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "AuctionLot",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "AuctionLot",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "AuctionLot",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "AuctionLot");

            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "AuctionLot");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AuctionLot");

            migrationBuilder.RenameColumn(
                name: "DateStart",
                table: "AuctionLot",
                newName: "Date");
        }
    }
}
