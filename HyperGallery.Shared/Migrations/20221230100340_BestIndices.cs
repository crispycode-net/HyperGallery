using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperGallery.Shared.Migrations
{
    public partial class BestIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Files_BestGuess",
                table: "Files",
                column: "BestGuess");

            migrationBuilder.CreateIndex(
                name: "IX_Files_BestGuessYear",
                table: "Files",
                column: "BestGuessYear");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_BestGuess",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_BestGuessYear",
                table: "Files");
        }
    }
}
