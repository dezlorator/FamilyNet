using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class editbase11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_BaseItemType_AuctionLotItemTypeID",
                table: "BaseItemType");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_BaseItemType_AuctionLotItemTypeID1",
                table: "BaseItemType");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_BaseItemType_DonationItemTypeID",
                table: "BaseItemType");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_BaseItemType_DonationItemTypeID1",
                table: "BaseItemType");

            migrationBuilder.DropIndex(
                name: "IX_BaseItemType_AuctionLotItemTypeID",
                table: "BaseItemType");

            migrationBuilder.DropIndex(
                name: "IX_BaseItemType_AuctionLotItemTypeID1",
                table: "BaseItemType");

            migrationBuilder.DropIndex(
                name: "IX_BaseItemType_DonationItemTypeID",
                table: "BaseItemType");

            migrationBuilder.DropIndex(
                name: "IX_BaseItemType_DonationItemTypeID1",
                table: "BaseItemType");

            migrationBuilder.DropColumn(
                name: "AuctionLotItemTypeID",
                table: "BaseItemType");

            migrationBuilder.DropColumn(
                name: "AuctionLotItemTypeID1",
                table: "BaseItemType");

            migrationBuilder.DropColumn(
                name: "DonationItemTypeID",
                table: "BaseItemType");

            migrationBuilder.RenameColumn(
                name: "DonationItemTypeID1",
                table: "BaseItemType",
                newName: "ChildID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_ChildID",
                table: "BaseItemType",
                column: "ChildID",
                unique: true,
                filter: "[ChildID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItemType_BaseItemType_ChildID",
                table: "BaseItemType",
                column: "ChildID",
                principalTable: "BaseItemType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_BaseItemType_ChildID",
                table: "BaseItemType");

            migrationBuilder.DropIndex(
                name: "IX_BaseItemType_ChildID",
                table: "BaseItemType");

            migrationBuilder.RenameColumn(
                name: "ChildID",
                table: "BaseItemType",
                newName: "DonationItemTypeID1");

            migrationBuilder.AddColumn<int>(
                name: "AuctionLotItemTypeID",
                table: "BaseItemType",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AuctionLotItemTypeID1",
                table: "BaseItemType",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DonationItemTypeID",
                table: "BaseItemType",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_AuctionLotItemTypeID",
                table: "BaseItemType",
                column: "AuctionLotItemTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_AuctionLotItemTypeID1",
                table: "BaseItemType",
                column: "AuctionLotItemTypeID1");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_DonationItemTypeID",
                table: "BaseItemType",
                column: "DonationItemTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_DonationItemTypeID1",
                table: "BaseItemType",
                column: "DonationItemTypeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItemType_BaseItemType_AuctionLotItemTypeID",
                table: "BaseItemType",
                column: "AuctionLotItemTypeID",
                principalTable: "BaseItemType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItemType_BaseItemType_AuctionLotItemTypeID1",
                table: "BaseItemType",
                column: "AuctionLotItemTypeID1",
                principalTable: "BaseItemType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItemType_BaseItemType_DonationItemTypeID",
                table: "BaseItemType",
                column: "DonationItemTypeID",
                principalTable: "BaseItemType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItemType_BaseItemType_DonationItemTypeID1",
                table: "BaseItemType",
                column: "DonationItemTypeID1",
                principalTable: "BaseItemType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
