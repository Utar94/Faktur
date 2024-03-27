using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class IndexSkuNormalized : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_StoreId_Sku",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId_SkuNormalized",
                table: "Products",
                columns: new[] { "StoreId", "SkuNormalized" },
                unique: true,
                filter: "[SkuNormalized] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_StoreId_SkuNormalized",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId_Sku",
                table: "Products",
                columns: new[] { "StoreId", "Sku" },
                unique: true,
                filter: "[Sku] IS NOT NULL");
        }
    }
}
