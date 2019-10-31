using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class fairCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "UserId",
                table: "AuctionLot");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "AuctionLot",
                newName: "Avatar");

            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "AuctionLot",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "AuctionLot",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AuctionLot");

            migrationBuilder.RenameColumn(
                name: "Avatar",
                table: "AuctionLot",
                newName: "UserId1");

            migrationBuilder.AlterColumn<string>(
                name: "UserId1",
                table: "AuctionLot",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AuctionLot",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PersonID = table.Column<int>(nullable: true),
                    PersonType = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
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
    }
}
