using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KETEBAN.DATA.Migrations
{
    /// <inheritdoc />
    public partial class changeRelMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_BookLanguageId",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookLanguageId",
                table: "Books",
                column: "BookLanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_BookLanguageId",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookLanguageId",
                table: "Books",
                column: "BookLanguageId",
                unique: true);
        }
    }
}
