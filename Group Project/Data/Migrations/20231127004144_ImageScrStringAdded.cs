using Microsoft.EntityFrameworkCore.Migrations;

namespace Group_Project.Data.Migrations
{
    public partial class ImageScrStringAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageSrc",
                table: "Movie",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageSrc",
                table: "Movie");
        }
    }
}
