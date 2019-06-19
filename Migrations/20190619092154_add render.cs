using Microsoft.EntityFrameworkCore.Migrations;

namespace Crawler.Migrations
{
    public partial class addrender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rendered",
                table: "News",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rendered",
                table: "News");
        }
    }
}
