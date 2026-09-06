using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "varchar(200)",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Products_varchar(200)",
                table: "Products",
                newName: "IX_Products_Name");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "varchar(200)");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Name",
                table: "Products",
                newName: "IX_Products_varchar(200)");

            migrationBuilder.AlterColumn<string>(
                name: "varchar(200)",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)");
        }
    }
}
