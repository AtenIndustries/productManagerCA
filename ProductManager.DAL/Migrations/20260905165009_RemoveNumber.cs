using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Number",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "varchar(200)",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Products_varchar(200)",
                table: "Products",
                column: "varchar(200)",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_varchar(200)",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "varchar(200)",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
            
            //To avoid index duplication errors due to default value being 0
            migrationBuilder.Sql(@"
                WITH CTE AS (
                    SELECT Number, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) as RowNum
                    FROM dbo.Products
                )
                UPDATE CTE SET Number = RowNum;
            ");
            migrationBuilder.CreateIndex(
                name: "IX_Products_Number",
                table: "Products",
                column: "Number",
                unique: true);
        }
    }
}
