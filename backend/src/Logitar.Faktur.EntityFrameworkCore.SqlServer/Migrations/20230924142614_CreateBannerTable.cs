using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateBannerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    BannerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.BannerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banners_AggregateId",
                table: "Banners",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banners_CreatedBy",
                table: "Banners",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_CreatedOn",
                table: "Banners",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_DisplayName",
                table: "Banners",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_UpdatedBy",
                table: "Banners",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_UpdatedOn",
                table: "Banners",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_Version",
                table: "Banners",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");
        }
    }
}
