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

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Departments_DepartmentId",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Articles_ArticleId",
                table: "Products",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Departments_DepartmentId",
                table: "Products",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Articles_ArticleId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Departments_DepartmentId",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Articles_ArticleId",
                table: "Products",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Departments_DepartmentId",
                table: "Products",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId");
        }
    }
}
