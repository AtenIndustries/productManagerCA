using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSeqForId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_Products", "Products");
            migrationBuilder.DropColumn("Id", "Products");
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateSequence<int>(
                name: "ProductIds",
                schema: "dbo",
                startValue: 100000L,
                maxValue: 999999L);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR dbo.ProductIds")
                .Annotation("Relational:DefaultConstraintName", "DF_Products_Id");

            migrationBuilder.AddPrimaryKey("PK_Products", "Products", "Id");

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [Products] DROP CONSTRAINT [DF_Products_Id];");
            migrationBuilder.DropSequence(
                name: "ProductIds",
                schema: "dbo");

            migrationBuilder.DropPrimaryKey("PK_Products", "Products");
            migrationBuilder.DropColumn("Id", "Products");
        }
    }
}
