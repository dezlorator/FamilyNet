using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class AddAddressID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orphanages_Address_AddressID",
                table: "Orphanages");

            migrationBuilder.AlterColumn<int>(
                name: "AddressID",
                table: "Orphanages",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orphanages_Address_AddressID",
                table: "Orphanages",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orphanages_Address_AddressID",
                table: "Orphanages");

            migrationBuilder.AlterColumn<int>(
                name: "AddressID",
                table: "Orphanages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Orphanages_Address_AddressID",
                table: "Orphanages",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
