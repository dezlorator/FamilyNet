using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNet.Migrations
{
    public partial class DeletechildinBaseItemType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_BaseItemType_ChildID",
                table: "BaseItemType");

            migrationBuilder.DropIndex(
                name: "IX_BaseItemType_ChildID",
                table: "BaseItemType");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Donations",
                newName: "LastDateWhenStatusChanged");

            migrationBuilder.RenameColumn(
                name: "ChildID",
                table: "BaseItemType",
                newName: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseItemType_ParentID",
                table: "BaseItemType",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItemType_BaseItemType_ParentID",
                table: "BaseItemType",
                column: "ParentID",
                principalTable: "BaseItemType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseItemType_BaseItemType_ParentID",
                table: "BaseItemType");

            migrationBuilder.DropIndex(
                name: "IX_BaseItemType_ParentID",
                table: "BaseItemType");

            migrationBuilder.RenameColumn(
                name: "LastDateWhenStatusChanged",
                table: "Donations",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "ParentID",
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
    }
}
