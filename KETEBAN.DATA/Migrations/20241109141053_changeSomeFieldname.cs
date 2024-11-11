using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KETEBAN.DATA.Migrations
{
    /// <inheritdoc />
    public partial class changeSomeFieldname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_loans_Students_StudentId",
                table: "loans");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "loans",
                newName: "StudentNumber");

            migrationBuilder.RenameColumn(
                name: "BorrowDate",
                table: "loans",
                newName: "DateofLoan");

            migrationBuilder.RenameIndex(
                name: "IX_loans_StudentId",
                table: "loans",
                newName: "IX_loans_StudentNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_loans_Students_StudentNumber",
                table: "loans",
                column: "StudentNumber",
                principalTable: "Students",
                principalColumn: "StudentNum",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_loans_Students_StudentNumber",
                table: "loans");

            migrationBuilder.RenameColumn(
                name: "StudentNumber",
                table: "loans",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "DateofLoan",
                table: "loans",
                newName: "BorrowDate");

            migrationBuilder.RenameIndex(
                name: "IX_loans_StudentNumber",
                table: "loans",
                newName: "IX_loans_StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_loans_Students_StudentId",
                table: "loans",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentNum",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
