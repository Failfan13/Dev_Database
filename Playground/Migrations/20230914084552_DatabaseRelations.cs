using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF__Console.Migrations
{
    public partial class DatabaseRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UsersInGroups_GroupId",
                table: "UsersInGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInGroups_UserId",
                table: "UsersInGroups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInGroups_Groups_GroupId",
                table: "UsersInGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInGroups_Users_UserId",
                table: "UsersInGroups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersInGroups_Groups_GroupId",
                table: "UsersInGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersInGroups_Users_UserId",
                table: "UsersInGroups");

            migrationBuilder.DropIndex(
                name: "IX_UsersInGroups_GroupId",
                table: "UsersInGroups");

            migrationBuilder.DropIndex(
                name: "IX_UsersInGroups_UserId",
                table: "UsersInGroups");
        }
    }
}
