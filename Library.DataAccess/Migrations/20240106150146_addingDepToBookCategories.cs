using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addingDepToBookCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_BookSubjects_BookSubjectId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_BookSubjectId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "BookSubjectId",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SubjectId",
                table: "Categories",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_BookSubjects_SubjectId",
                table: "Categories",
                column: "SubjectId",
                principalTable: "BookSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_BookSubjects_SubjectId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SubjectId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "BookSubjectId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_BookSubjectId",
                table: "Categories",
                column: "BookSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_BookSubjects_BookSubjectId",
                table: "Categories",
                column: "BookSubjectId",
                principalTable: "BookSubjects",
                principalColumn: "Id");
        }
    }
}
