using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "Name", "Quantity", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 100000, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "App", "Cheapest dishwasher ever", "Dishwasher", 12, null, null },
                    { 100001, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "App", "Wooden slignshot", "Slingshot", 1, null, null },
                    { 100002, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "App", "Action figure", "DreadPool Action Figure", 8, null, null },
                    { 100004, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "App", "Mix of fruits", "Bag of fruits", 25, null, null },
                    { 100005, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "App", "Mix of nuts", "Bag of nuts", 20, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100000);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100001);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100002);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100004);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100005);
        }
    }
}
