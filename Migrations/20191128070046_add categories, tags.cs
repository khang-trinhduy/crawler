using Microsoft.EntityFrameworkCore.Migrations;

namespace Crawler.Migrations
{
    public partial class addcategoriestags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Categories",
                table: "News",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "News",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "Categories",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "News");
        }
    }
}
