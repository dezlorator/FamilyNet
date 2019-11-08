using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyNetServer.Migrations
{
    public partial class ActivitiesFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Awards_Activities_ChildrenActivityID",
                table: "Awards");

            migrationBuilder.RenameColumn(
                name: "ChildrenActivityID",
                table: "Awards",
                newName: "ChildActivityID");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_ChildrenActivityID",
                table: "Awards",
                newName: "IX_Awards_ChildActivityID");

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_Activities_ChildActivityID",
                table: "Awards",
                column: "ChildActivityID",
                principalTable: "Activities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Awards_Activities_ChildActivityID",
                table: "Awards");

            migrationBuilder.RenameColumn(
                name: "ChildActivityID",
                table: "Awards",
                newName: "ChildrenActivityID");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_ChildActivityID",
                table: "Awards",
                newName: "IX_Awards_ChildrenActivityID");

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_Activities_ChildrenActivityID",
                table: "Awards",
                column: "ChildrenActivityID",
                principalTable: "Activities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
