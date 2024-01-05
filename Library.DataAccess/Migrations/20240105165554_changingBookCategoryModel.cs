using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changingBookCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookSubjectId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookSubjects", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_BookSubjects_BookSubjectId",
                table: "Categories");

            migrationBuilder.DropTable(
                name: "BookSubjects");

            migrationBuilder.DropIndex(
                name: "IX_Categories_BookSubjectId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "BookSubjectId",
                table: "Categories");
        }
    }
}
