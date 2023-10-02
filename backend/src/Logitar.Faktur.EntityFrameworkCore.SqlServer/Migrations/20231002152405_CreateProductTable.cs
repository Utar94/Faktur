using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_StoreId_NumberNormalized",
                table: "Departments");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    SkuNormalized = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Flags = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UnitPrice = table.Column<double>(type: "float", nullable: true),
                    UnitType = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId");
                    table.ForeignKey(
                        name: "FK_Products_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_StoreId_NumberNormalized",
                table: "Departments",
                columns: new[] { "StoreId", "NumberNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ArticleId",
                table: "Products",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedBy",
                table: "Products",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedOn",
                table: "Products",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DepartmentId",
                table: "Products",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DisplayName",
                table: "Products",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Flags",
                table: "Products",
                column: "Flags");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId_ArticleId",
                table: "Products",
                columns: new[] { "StoreId", "ArticleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId_SkuNormalized",
                table: "Products",
                columns: new[] { "StoreId", "SkuNormalized" },
                unique: true,
                filter: "[SkuNormalized] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitPrice",
                table: "Products",
                column: "UnitPrice");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitType",
                table: "Products",
                column: "UnitType");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedBy",
                table: "Products",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedOn",
                table: "Products",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Version",
                table: "Products",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Departments_StoreId_NumberNormalized",
                table: "Departments");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_StoreId_NumberNormalized",
                table: "Departments",
                columns: new[] { "StoreId", "NumberNormalized" });
        }
    }
}
