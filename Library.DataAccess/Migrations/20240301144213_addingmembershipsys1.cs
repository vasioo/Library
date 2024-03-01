using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addingmembershipsys1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NeededAmountOfPoints",
                table: "Memberships",
                newName: "StartingNeededAmountOfPoints");

            migrationBuilder.AddColumn<int>(
                name: "EndAmountOfPoints",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndAmountOfPoints",
                table: "Memberships");

            migrationBuilder.RenameColumn(
                name: "StartingNeededAmountOfPoints",
                table: "Memberships",
                newName: "NeededAmountOfPoints");
        }
    }
}
