using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class auctionUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "AuctionLot",
                newName: "DateStart");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "AuctionLot",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AuctionLot",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "AuctionLot",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    PersonID = table.Column<int>(nullable: true),
                    PersonType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuctionLot_UserId1",
                table: "AuctionLot",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionLot_ApplicationUser_UserId1",
                table: "AuctionLot",
                column: "UserId1",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionLot_ApplicationUser_UserId1",
                table: "AuctionLot");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_AuctionLot_UserId1",
                table: "AuctionLot");

            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "AuctionLot");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AuctionLot");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "AuctionLot");

            migrationBuilder.RenameColumn(
                name: "DateStart",
                table: "AuctionLot",
                newName: "Date");
        }
    }
}
