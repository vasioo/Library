using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addingBookEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserLeasedBooks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "UserLeasedBooks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AmountOfBooks",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserLeasedBooks_UserId",
                table: "UserLeasedBooks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLeasedBooks_AspNetUsers_UserId",
                table: "UserLeasedBooks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLeasedBooks_AspNetUsers_UserId",
                table: "UserLeasedBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserLeasedBooks_UserId",
                table: "UserLeasedBooks");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "UserLeasedBooks");

            migrationBuilder.DropColumn(
                name: "AmountOfBooks",
                table: "Books");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserLeasedBooks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
