using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class FixedIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Articles_ArticleId",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Articles_ArticleId",
                table: "Products",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Articles_ArticleId",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Articles_ArticleId",
                table: "Products",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
