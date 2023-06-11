using Microsoft.EntityFrameworkCore.Migrations;

namespace HyperGallery.Shared.Migrations
{
    public partial class ScanErrors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScanErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Error = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    SourcePath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanErrors", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScanErrors");
        }
    }
}
