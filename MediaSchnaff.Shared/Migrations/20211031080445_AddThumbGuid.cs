using Microsoft.EntityFrameworkCore.Migrations;

namespace MediaSchnaff.Shared.Migrations
{
    public partial class AddThumbGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbGuid",
                table: "Files",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbGuid",
                table: "Files");
        }
    }
}
