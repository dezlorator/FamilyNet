using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class many2many : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionLot_AuctionLotItem_AuctionLotItemID",
                table: "AuctionLot");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_AuctionLotItem_ItemID",
                table: "BaseItemType");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_DonationItem_DonationItemType_ItemID",
                table: "BaseItemType");

            migrationBuilder.DropForeignKey(
                name: "FK_Donations_DonationItem_DonationItemID",
                table: "Donations");

            migrationBuilder.DropTable(
                name: "AuctionLotItem");

            migrationBuilder.DropIndex(
                name: "IX_BaseItemType_DonationItemType_ItemID",
                table: "BaseItemType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonationItem",
                table: "DonationItem");

            migrationBuilder.DropColumn(
                name: "DonationItemType_ItemID",
                table: "BaseItemType");

            migrationBuilder.RenameTable(
                name: "DonationItem",
                newName: "BaseItem");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "BaseItem",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaseItem",
                table: "BaseItem",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "TypeBaseItem",
                columns: table => new
                {
                    ItemID = table.Column<int>(nullable: false),
                    TypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeBaseItem", x => new { x.ItemID, x.TypeID });
                    table.ForeignKey(
                        name: "FK_TypeBaseItem_BaseItem_ItemID",
                        column: x => x.ItemID,
                        principalTable: "BaseItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TypeBaseItem_BaseItemType_TypeID",
                        column: x => x.TypeID,
                        principalTable: "BaseItemType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TypeBaseItem_TypeID",
                table: "TypeBaseItem",
                column: "TypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionLot_BaseItem_AuctionLotItemID",
                table: "AuctionLot",
                column: "AuctionLotItemID",
                principalTable: "BaseItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItemType_BaseItem_ItemID",
                table: "BaseItemType",
                column: "ItemID",
                principalTable: "BaseItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_BaseItem_DonationItemID",
                table: "Donations",
                column: "DonationItemID",
                principalTable: "BaseItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionLot_BaseItem_AuctionLotItemID",
                table: "AuctionLot");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_BaseItem_ItemID",
                table: "BaseItemType");

            migrationBuilder.DropForeignKey(
                name: "FK_Donations_BaseItem_DonationItemID",
                table: "Donations");

            migrationBuilder.DropTable(
                name: "TypeBaseItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaseItem",
                table: "BaseItem");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "BaseItem");

            migrationBuilder.RenameTable(
                name: "BaseItem",
                newName: "DonationItem");

            migrationBuilder.AddColumn<int>(
                name: "DonationItemType_ItemID",
                table: "BaseItemType",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonationItem",
                table: "DonationItem",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "AuctionLotItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Price = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionLotItem", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_DonationItemType_ItemID",
                table: "BaseItemType",
                column: "DonationItemType_ItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionLot_AuctionLotItem_AuctionLotItemID",
                table: "AuctionLot",
                column: "AuctionLotItemID",
                principalTable: "AuctionLotItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItemType_AuctionLotItem_ItemID",
                table: "BaseItemType",
                column: "ItemID",
                principalTable: "AuctionLotItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItemType_DonationItem_DonationItemType_ItemID",
                table: "BaseItemType",
                column: "DonationItemType_ItemID",
                principalTable: "DonationItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_DonationItem_DonationItemID",
                table: "Donations",
                column: "DonationItemID",
                principalTable: "DonationItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
