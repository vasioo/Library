using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addingBanningSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BanStatus",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanStatus",
                table: "AspNetUsers");
        }
    }
}
