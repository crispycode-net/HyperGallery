using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaSchnaff.Shared.Migrations
{
    public partial class MimeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BestGuessMimeType",
                table: "Files",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestGuessMimeType",
                table: "Files");
        }
    }
}
