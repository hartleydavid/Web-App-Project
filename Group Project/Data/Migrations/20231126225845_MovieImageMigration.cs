using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Group_Project.Data.Migrations
{
    public partial class MovieImageMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Movie",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Movie");
        }
    }
}
