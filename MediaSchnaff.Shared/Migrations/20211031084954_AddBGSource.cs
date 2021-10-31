using Microsoft.EntityFrameworkCore.Migrations;

namespace MediaSchnaff.Shared.Migrations
{
    public partial class AddBGSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BestGuessSource",
                table: "Files",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestGuessSource",
                table: "Files");
        }
    }
}
