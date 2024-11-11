using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KETEBAN.DATA.Migrations
{
    /// <inheritdoc />
    public partial class addtableMIG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loan_Books_BookId",
                table: "Loan");

            migrationBuilder.DropForeignKey(
                name: "FK_Loan_Students_StudentId",
                table: "Loan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Loan",
                table: "Loan");

            migrationBuilder.RenameTable(
                name: "Loan",
                newName: "loans");

            migrationBuilder.RenameIndex(
                name: "IX_Loan_StudentId",
                table: "loans",
                newName: "IX_loans_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Loan_BookId",
                table: "loans",
                newName: "IX_loans_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_loans",
                table: "loans",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_loans_Books_BookId",
                table: "loans",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_loans_Students_StudentId",
                table: "loans",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentNum",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_loans_Books_BookId",
                table: "loans");

            migrationBuilder.DropForeignKey(
                name: "FK_loans_Students_StudentId",
                table: "loans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_loans",
                table: "loans");

            migrationBuilder.RenameTable(
                name: "loans",
                newName: "Loan");

            migrationBuilder.RenameIndex(
                name: "IX_loans_StudentId",
                table: "Loan",
                newName: "IX_Loan_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_loans_BookId",
                table: "Loan",
                newName: "IX_Loan_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Loan",
                table: "Loan",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loan_Books_BookId",
                table: "Loan",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loan_Students_StudentId",
                table: "Loan",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentNum",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
