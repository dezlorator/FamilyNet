using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class Init : Migration
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
                name: "Contacts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ID);
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
                name: "FullName",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Surname = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Patronymic = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FullName", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Orphanages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    AddressID = table.Column<int>(nullable: true),
                    Rating = table.Column<float>(nullable: false),
                    Avatar = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orphanages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orphanages_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaseItemType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    AuctionLotItemTypeID = table.Column<int>(nullable: true),
                    AuctionLotItemTypeID1 = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    DonationItemTypeID = table.Column<int>(nullable: true),
                    DonationItemTypeID1 = table.Column<int>(nullable: true),
                    AuctionLotItemID = table.Column<int>(nullable: true),
                    DonationItemID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseItemType", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BaseItemType_AuctionLotItem_AuctionLotItemID",
                        column: x => x.AuctionLotItemID,
                        principalTable: "AuctionLotItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseItemType_BaseItemType_AuctionLotItemTypeID",
                        column: x => x.AuctionLotItemTypeID,
                        principalTable: "BaseItemType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseItemType_BaseItemType_AuctionLotItemTypeID1",
                        column: x => x.AuctionLotItemTypeID1,
                        principalTable: "BaseItemType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseItemType_BaseItemType_DonationItemTypeID",
                        column: x => x.DonationItemTypeID,
                        principalTable: "BaseItemType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseItemType_BaseItemType_DonationItemTypeID1",
                        column: x => x.DonationItemTypeID1,
                        principalTable: "BaseItemType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseItemType_DonationItem_DonationItemID",
                        column: x => x.DonationItemID,
                        principalTable: "DonationItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharityMakers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullNameID = table.Column<int>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    AddressID = table.Column<int>(nullable: true),
                    ContactsID = table.Column<int>(nullable: true),
                    Rating = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharityMakers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CharityMakers_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityMakers_Contacts_ContactsID",
                        column: x => x.ContactsID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharityMakers_FullName_FullNameID",
                        column: x => x.FullNameID,
                        principalTable: "FullName",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Volunteers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullNameID = table.Column<int>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    AddressID = table.Column<int>(nullable: true),
                    ContactsID = table.Column<int>(nullable: true),
                    Rating = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volunteers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Volunteers_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Volunteers_Contacts_ContactsID",
                        column: x => x.ContactsID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Volunteers_FullName_FullNameID",
                        column: x => x.FullNameID,
                        principalTable: "FullName",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orphans",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullNameID = table.Column<int>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    AddressID = table.Column<int>(nullable: true),
                    ContactsID = table.Column<int>(nullable: true),
                    Rating = table.Column<float>(nullable: false),
                    OrphanageID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orphans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orphans_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orphans_Contacts_ContactsID",
                        column: x => x.ContactsID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orphans_FullName_FullNameID",
                        column: x => x.FullNameID,
                        principalTable: "FullName",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orphans_Orphanages_OrphanageID",
                        column: x => x.OrphanageID,
                        principalTable: "Orphanages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Representatives",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullNameID = table.Column<int>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: false),
                    AddressID = table.Column<int>(nullable: true),
                    ContactsID = table.Column<int>(nullable: true),
                    Rating = table.Column<float>(nullable: false),
                    OrphanageID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Representatives", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Representatives_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Representatives_Contacts_ContactsID",
                        column: x => x.ContactsID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Representatives_FullName_FullNameID",
                        column: x => x.FullNameID,
                        principalTable: "FullName",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Representatives_Orphanages_OrphanageID",
                        column: x => x.OrphanageID,
                        principalTable: "Orphanages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DonationItemID = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    CharityMakerID = table.Column<int>(nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "AuctionLot",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuctionLotItemID = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
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
                name: "IX_AuctionLot_AuctionLotItemID",
                table: "AuctionLot",
                column: "AuctionLotItemID");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionLot_OrphanID",
                table: "AuctionLot",
                column: "OrphanID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_AuctionLotItemID",
                table: "BaseItemType",
                column: "AuctionLotItemID");

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

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_DonationItemID",
                table: "BaseItemType",
                column: "DonationItemID");

            migrationBuilder.CreateIndex(
                name: "IX_CharityMakers_AddressID",
                table: "CharityMakers",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_CharityMakers_ContactsID",
                table: "CharityMakers",
                column: "ContactsID");

            migrationBuilder.CreateIndex(
                name: "IX_CharityMakers_FullNameID",
                table: "CharityMakers",
                column: "FullNameID");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_CharityMakerID",
                table: "Donations",
                column: "CharityMakerID");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DonationItemID",
                table: "Donations",
                column: "DonationItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Orphanages_AddressID",
                table: "Orphanages",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Orphans_AddressID",
                table: "Orphans",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Orphans_ContactsID",
                table: "Orphans",
                column: "ContactsID");

            migrationBuilder.CreateIndex(
                name: "IX_Orphans_FullNameID",
                table: "Orphans",
                column: "FullNameID");

            migrationBuilder.CreateIndex(
                name: "IX_Orphans_OrphanageID",
                table: "Orphans",
                column: "OrphanageID");

            migrationBuilder.CreateIndex(
                name: "IX_Representatives_AddressID",
                table: "Representatives",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Representatives_ContactsID",
                table: "Representatives",
                column: "ContactsID");

            migrationBuilder.CreateIndex(
                name: "IX_Representatives_FullNameID",
                table: "Representatives",
                column: "FullNameID");

            migrationBuilder.CreateIndex(
                name: "IX_Representatives_OrphanageID",
                table: "Representatives",
                column: "OrphanageID");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_AddressID",
                table: "Volunteers",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_ContactsID",
                table: "Volunteers",
                column: "ContactsID");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_FullNameID",
                table: "Volunteers",
                column: "FullNameID");
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
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "FullName");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
