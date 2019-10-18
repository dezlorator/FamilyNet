using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TypeBaseItem_BaseItem_ItemID",
                table: "TypeBaseItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TypeBaseItem_BaseItemType_TypeID",
                table: "TypeBaseItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypeBaseItem",
                table: "TypeBaseItem");

            migrationBuilder.RenameTable(
                name: "TypeBaseItem",
                newName: "TypeBaseItems");

            migrationBuilder.RenameIndex(
                name: "IX_TypeBaseItem_TypeID",
                table: "TypeBaseItems",
                newName: "IX_TypeBaseItems_TypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypeBaseItems",
                table: "TypeBaseItems",
                columns: new[] { "ItemID", "TypeID" });

            migrationBuilder.AddForeignKey(
                name: "FK_TypeBaseItems_BaseItem_ItemID",
                table: "TypeBaseItems",
                column: "ItemID",
                principalTable: "BaseItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TypeBaseItems_BaseItemType_TypeID",
                table: "TypeBaseItems",
                column: "TypeID",
                principalTable: "BaseItemType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TypeBaseItems_BaseItem_ItemID",
                table: "TypeBaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TypeBaseItems_BaseItemType_TypeID",
                table: "TypeBaseItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypeBaseItems",
                table: "TypeBaseItems");

            migrationBuilder.RenameTable(
                name: "TypeBaseItems",
                newName: "TypeBaseItem");

            migrationBuilder.RenameIndex(
                name: "IX_TypeBaseItems_TypeID",
                table: "TypeBaseItem",
                newName: "IX_TypeBaseItem_TypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypeBaseItem",
                table: "TypeBaseItem",
                columns: new[] { "ItemID", "TypeID" });

            migrationBuilder.AddForeignKey(
                name: "FK_TypeBaseItem_BaseItem_ItemID",
                table: "TypeBaseItem",
                column: "ItemID",
                principalTable: "BaseItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TypeBaseItem_BaseItemType_TypeID",
                table: "TypeBaseItem",
                column: "TypeID",
                principalTable: "BaseItemType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
