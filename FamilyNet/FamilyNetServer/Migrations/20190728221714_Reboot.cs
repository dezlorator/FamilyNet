using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class Reboot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Country = table.Column<string>(nullable: false),
                    Region = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: false),
                    House = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AuctionLotItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionLotItem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DonationItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationItem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CharityMakers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName_Name = table.Column<string>(nullable: false),
                    FullName_Surname = table.Column<string>(nullable: false),
                    FullName_Patronymic = table.Column<string>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    Rating = table.Column<float>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    EmailID = table.Column<int>(nullable: false),
                    AddressID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityMakers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CharityMakers_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Orphanages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    AdressID = table.Column<int>(nullable: true),
                    Rating = table.Column<float>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    MapCoordX = table.Column<float>(nullable: true),
                    MapCoordY = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orphanages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orphanages_Address_AdressID",
                        column: x => x.AdressID,
                        principalTable: "Address",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Volunteers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName_Name = table.Column<string>(nullable: false),
                    FullName_Surname = table.Column<string>(nullable: false),
                    FullName_Patronymic = table.Column<string>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    Rating = table.Column<float>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    EmailID = table.Column<int>(nullable: false),
                    AddressID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volunteers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Volunteers_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BaseItemType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    ChildID = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ItemID = table.Column<int>(nullable: true),
                    DonationItemType_ItemID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseItemType", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BaseItemType_AuctionLotItem_ItemID",
                        column: x => x.ItemID,
                        principalTable: "AuctionLotItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseItemType_BaseItemType_ChildID",
                        column: x => x.ChildID,
                        principalTable: "BaseItemType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseItemType_DonationItem_DonationItemType_ItemID",
                        column: x => x.DonationItemType_ItemID,
                        principalTable: "DonationItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DonationItemID = table.Column<int>(nullable: true),
                    IsRequest = table.Column<bool>(nullable: false),
                    CharityMakerID = table.Column<int>(nullable: true),
                    OrphanageID = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Donations_CharityMakers_CharityMakerID",
                        column: x => x.CharityMakerID,
                        principalTable: "CharityMakers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Donations_DonationItem_DonationItemID",
                        column: x => x.DonationItemID,
                        principalTable: "DonationItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Donations_Orphanages_OrphanageID",
                        column: x => x.OrphanageID,
                        principalTable: "Orphanages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orphans",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName_Name = table.Column<string>(nullable: false),
                    FullName_Surname = table.Column<string>(nullable: false),
                    FullName_Patronymic = table.Column<string>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    Rating = table.Column<float>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    EmailID = table.Column<int>(nullable: false),
                    OrphanageID = table.Column<int>(nullable: false),
                    Confirmation = table.Column<bool>(nullable: false),
                    ChildInOrphanage = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orphans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orphans_Orphanages_OrphanageID",
                        column: x => x.OrphanageID,
                        principalTable: "Orphanages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Representatives",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName_Name = table.Column<string>(nullable: false),
                    FullName_Surname = table.Column<string>(nullable: false),
                    FullName_Patronymic = table.Column<string>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    Rating = table.Column<float>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    EmailID = table.Column<int>(nullable: false),
                    OrphanageID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Representatives", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Representatives_Orphanages_OrphanageID",
                        column: x => x.OrphanageID,
                        principalTable: "Orphanages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuctionLot",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    AuctionLotItemID = table.Column<int>(nullable: true),
                    OrphanID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionLot", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AuctionLot_AuctionLotItem_AuctionLotItemID",
                        column: x => x.AuctionLotItemID,
                        principalTable: "AuctionLotItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuctionLot_Orphans_OrphanID",
                        column: x => x.OrphanID,
                        principalTable: "Orphans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_City",
                table: "Address",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionLot_AuctionLotItemID",
                table: "AuctionLot",
                column: "AuctionLotItemID");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionLot_OrphanID",
                table: "AuctionLot",
                column: "OrphanID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_ItemID",
                table: "BaseItemType",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_ChildID",
                table: "BaseItemType",
                column: "ChildID",
                unique: true,
                filter: "[ChildID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_DonationItemType_ItemID",
                table: "BaseItemType",
                column: "DonationItemType_ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_CharityMakers_AddressID",
                table: "CharityMakers",
                column: "AddressID",
                unique: true,
                filter: "[AddressID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_CharityMakerID",
                table: "Donations",
                column: "CharityMakerID");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DonationItemID",
                table: "Donations",
                column: "DonationItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_OrphanageID",
                table: "Donations",
                column: "OrphanageID");

            migrationBuilder.CreateIndex(
                name: "IX_Orphanages_AdressID",
                table: "Orphanages",
                column: "AdressID",
                unique: true,
                filter: "[AdressID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orphans_OrphanageID",
                table: "Orphans",
                column: "OrphanageID");

            migrationBuilder.CreateIndex(
                name: "IX_Representatives_OrphanageID",
                table: "Representatives",
                column: "OrphanageID");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_AddressID",
                table: "Volunteers",
                column: "AddressID",
                unique: true,
                filter: "[AddressID] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionLot");

            migrationBuilder.DropTable(
                name: "BaseItemType");

            migrationBuilder.DropTable(
                name: "Donations");

            migrationBuilder.DropTable(
                name: "Representatives");

            migrationBuilder.DropTable(
                name: "Volunteers");

            migrationBuilder.DropTable(
                name: "Orphans");

            migrationBuilder.DropTable(
                name: "AuctionLotItem");

            migrationBuilder.DropTable(
                name: "CharityMakers");

            migrationBuilder.DropTable(
                name: "DonationItem");

            migrationBuilder.DropTable(
                name: "Orphanages");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
