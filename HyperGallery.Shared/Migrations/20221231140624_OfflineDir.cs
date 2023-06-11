using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperGallery.Shared.Migrations
{
    public partial class OfflineDir : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalMediaPath",
                table: "Files",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalMediaPath",
                table: "Files");
        }
    }
}
