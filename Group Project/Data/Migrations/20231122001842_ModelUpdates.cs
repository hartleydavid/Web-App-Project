using Microsoft.EntityFrameworkCore.Migrations;

namespace Group_Project.Data.Migrations
{
    public partial class ModelUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Movie_MovieId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Show_ShowId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_MovieId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ShowId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ShowId",
                table: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Comment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShowId",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_MovieId",
                table: "Comment",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ShowId",
                table: "Comment",
                column: "ShowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Movie_MovieId",
                table: "Comment",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Show_ShowId",
                table: "Comment",
                column: "ShowId",
                principalTable: "Show",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
