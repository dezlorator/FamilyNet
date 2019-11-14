using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class NewMigration : Migration
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
                    House = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Availabilities",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonID = table.Column<int>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    FreeHours = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsReserved = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    QuestName = table.Column<string>(nullable: true),
                    QuestID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availabilities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BaseItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseItem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Message = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    DonationId = table.Column<int>(nullable: false),
                    ReceiverRole = table.Column<int>(nullable: false),
                    ReceiverId = table.Column<int>(nullable: true),
                    SenderRole = table.Column<int>(nullable: false),
                    SenderId = table.Column<int>(nullable: true),
                    Rating = table.Column<double>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.ID);
                });

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
                    IsDeleted = table.Column<bool>(nullable: false),
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
                    IsDeleted = table.Column<bool>(nullable: false),
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
                    ParentID = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    ItemID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseItemType", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BaseItemType_BaseItem_ItemID",
                        column: x => x.ItemID,
                        principalTable: "BaseItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseItemType_BaseItemType_ParentID",
                        column: x => x.ParentID,
                        principalTable: "BaseItemType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
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
                    LocationID = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Orphanages_Location_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TypeBaseItems",
                columns: table => new
                {
                    ItemID = table.Column<int>(nullable: false),
                    TypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeBaseItems", x => new { x.ItemID, x.TypeID });
                    table.ForeignKey(
                        name: "FK_TypeBaseItems_BaseItem_ItemID",
                        column: x => x.ItemID,
                        principalTable: "BaseItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TypeBaseItems_BaseItemType_TypeID",
                        column: x => x.TypeID,
                        principalTable: "BaseItemType",
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
                    IsRequest = table.Column<bool>(nullable: false),
                    CharityMakerID = table.Column<int>(nullable: true),
                    OrphanageID = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    LastDateWhenStatusChanged = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
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
                        name: "FK_Donations_BaseItem_DonationItemID",
                        column: x => x.DonationItemID,
                        principalTable: "BaseItem",
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
                    IsDeleted = table.Column<bool>(nullable: false),
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
                    IsDeleted = table.Column<bool>(nullable: false),
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
                name: "Quests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    DonationID = table.Column<int>(nullable: true),
                    VolunteerID = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    FromDate = table.Column<DateTime>(nullable: false),
                    ToDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Quests_Donations_DonationID",
                        column: x => x.DonationID,
                        principalTable: "Donations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quests_Volunteers_VolunteerID",
                        column: x => x.VolunteerID,
                        principalTable: "Volunteers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ChildID = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Activities_Orphans_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Orphans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuctionLot",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    AuctionLotItemID = table.Column<int>(nullable: true),
                    OrphanID = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionLot", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AuctionLot_BaseItem_AuctionLotItemID",
                        column: x => x.AuctionLotItemID,
                        principalTable: "BaseItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuctionLot_Orphans_OrphanID",
                        column: x => x.OrphanID,
                        principalTable: "Orphans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Awards",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ChildActivityID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awards", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Awards_Activities_ChildActivityID",
                        column: x => x.ChildActivityID,
                        principalTable: "Activities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Activities_ChildID",
                table: "Activities",
                column: "ChildID");

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
                name: "IX_Awards_ChildActivityID",
                table: "Awards",
                column: "ChildActivityID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_ItemID",
                table: "BaseItemType",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_ParentID",
                table: "BaseItemType",
                column: "ParentID");

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
                column: "DonationItemID",
                unique: true,
                filter: "[DonationItemID] IS NOT NULL");

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
                name: "IX_Orphanages_LocationID",
                table: "Orphanages",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Orphans_OrphanageID",
                table: "Orphans",
                column: "OrphanageID");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_AuctionLotId",
                table: "Purchases",
                column: "AuctionLotId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_DonationID",
                table: "Quests",
                column: "DonationID");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_VolunteerID",
                table: "Quests",
                column: "VolunteerID");

            migrationBuilder.CreateIndex(
                name: "IX_Representatives_OrphanageID",
                table: "Representatives",
                column: "OrphanageID");

            migrationBuilder.CreateIndex(
                name: "IX_TypeBaseItems_TypeID",
                table: "TypeBaseItems",
                column: "TypeID");

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
                name: "Availabilities");

            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropTable(
                name: "Representatives");

            migrationBuilder.DropTable(
                name: "TypeBaseItems");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "AuctionLot");

            migrationBuilder.DropTable(
                name: "Donations");

            migrationBuilder.DropTable(
                name: "Volunteers");

            migrationBuilder.DropTable(
                name: "BaseItemType");

            migrationBuilder.DropTable(
                name: "Orphans");

            migrationBuilder.DropTable(
                name: "CharityMakers");

            migrationBuilder.DropTable(
                name: "BaseItem");

            migrationBuilder.DropTable(
                name: "Orphanages");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
