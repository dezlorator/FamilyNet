using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class editbase111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharityMakers_Adress_AddressID",
                table: "CharityMakers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orphanages_Adress_AdressID",
                table: "Orphanages");

            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_Adress_AddressID",
                table: "Volunteers");

            migrationBuilder.DropTable(
                name: "Adress");

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

            migrationBuilder.CreateIndex(
                name: "IX_Address_City",
                table: "Address",
                column: "City");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityMakers_Address_AddressID",
                table: "CharityMakers",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orphanages_Address_AdressID",
                table: "Orphanages",
                column: "AdressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_Address_AddressID",
                table: "Volunteers",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharityMakers_Address_AddressID",
                table: "CharityMakers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orphanages_Address_AdressID",
                table: "Orphanages");

            migrationBuilder.DropForeignKey(
                name: "FK_Volunteers_Address_AddressID",
                table: "Volunteers");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.CreateTable(
                name: "Adress",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: false),
                    House = table.Column<string>(nullable: false),
                    Region = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adress", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adress_City",
                table: "Adress",
                column: "City");

            migrationBuilder.AddForeignKey(
                name: "FK_CharityMakers_Adress_AddressID",
                table: "CharityMakers",
                column: "AddressID",
                principalTable: "Adress",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orphanages_Adress_AdressID",
                table: "Orphanages",
                column: "AdressID",
                principalTable: "Adress",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteers_Adress_AddressID",
                table: "Volunteers",
                column: "AddressID",
                principalTable: "Adress",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
