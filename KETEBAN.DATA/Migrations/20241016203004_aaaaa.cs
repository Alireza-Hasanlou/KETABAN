using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KETEBAN.DATA.Migrations
{
    /// <inheritdoc />
    public partial class aaaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarPath",
                table: "Librarians",
                newName: "AvatarName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarName",
                table: "Librarians",
                newName: "AvatarPath");
        }
    }
}
