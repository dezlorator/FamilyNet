using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class AddAddressID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orphanages_Adress_AdressID",
                table: "Orphanages");

            migrationBuilder.AlterColumn<int>(
                name: "AdressID",
                table: "Orphanages",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orphanages_Adress_AdressID",
                table: "Orphanages",
                column: "AdressID",
                principalTable: "Adress",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orphanages_Adress_AdressID",
                table: "Orphanages");

            migrationBuilder.AlterColumn<int>(
                name: "AdressID",
                table: "Orphanages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Orphanages_Adress_AdressID",
                table: "Orphanages",
                column: "AdressID",
                principalTable: "Adress",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
