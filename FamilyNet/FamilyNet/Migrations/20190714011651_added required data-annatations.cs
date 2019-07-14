using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class addedrequireddataannatations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharityMakers_Contacts_ContactsID",
                table: "CharityMakers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orphans_Contacts_ContactsID",
                table: "Orphans");

            migrationBuilder.DropForeignKey(
                name: "FK_Representatives_Contacts_ContactsID",
                table: "Representatives");

            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_Contacts_ContactsID",
                table: "Volunteers");

            migrationBuilder.AlterColumn<int>(
                name: "ContactsID",
                table: "Volunteers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContactsID",
                table: "Representatives",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContactsID",
                table: "Orphans",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContactsID",
                table: "CharityMakers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CharityMakers_Contacts_ContactsID",
                table: "CharityMakers",
                column: "ContactsID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orphans_Contacts_ContactsID",
                table: "Orphans",
                column: "ContactsID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Representatives_Contacts_ContactsID",
                table: "Representatives",
                column: "ContactsID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_Contacts_ContactsID",
                table: "Volunteers",
                column: "ContactsID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharityMakers_Contacts_ContactsID",
                table: "CharityMakers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orphans_Contacts_ContactsID",
                table: "Orphans");

            migrationBuilder.DropForeignKey(
                name: "FK_Representatives_Contacts_ContactsID",
                table: "Representatives");

            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_Contacts_ContactsID",
                table: "Volunteers");

            migrationBuilder.AlterColumn<int>(
                name: "ContactsID",
                table: "Volunteers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ContactsID",
                table: "Representatives",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ContactsID",
                table: "Orphans",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ContactsID",
                table: "CharityMakers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_CharityMakers_Contacts_ContactsID",
                table: "CharityMakers",
                column: "ContactsID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orphans_Contacts_ContactsID",
                table: "Orphans",
                column: "ContactsID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Representatives_Contacts_ContactsID",
                table: "Representatives",
                column: "ContactsID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_Contacts_ContactsID",
                table: "Volunteers",
                column: "ContactsID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
