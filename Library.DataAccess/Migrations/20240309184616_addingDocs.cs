using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addingDocs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4", "f7573d73-f9a4-495d-8425-257eaf275488" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f7573d73-f9a4-495d-8425-257eaf275488");

            migrationBuilder.DropColumn(
                name: "IsForAuthor",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BlogPosts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForAuthor",
                table: "BlogPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Points", "Position", "Salary", "SecurityStamp", "StartOfMembership", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f7573d73-f9a4-495d-8425-257eaf275488", 0, "c60e1396-7ea9-4c0b-bb6f-527b4950426c", "libraryadmin@gmail.com", false, "Vasil", "Orakov", false, null, "LIBRARYADMIN@GMAIL.COM", null, "library123!", "1234567890", false, 0, "", 0m, "3832cf4d-b898-4a39-9cbd-526f544bab16", new DateTime(2024, 3, 9, 17, 17, 47, 984, DateTimeKind.Local).AddTicks(8353), false, "library_1_admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "4", "f7573d73-f9a4-495d-8425-257eaf275488" });
        }
    }
}
