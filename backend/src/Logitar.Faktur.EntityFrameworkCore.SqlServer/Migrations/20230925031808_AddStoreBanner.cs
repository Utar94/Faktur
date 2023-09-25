using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreBanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BannerId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BannerId",
                table: "Stores",
                column: "BannerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Banners_BannerId",
                table: "Stores",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "BannerId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Banners_BannerId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_BannerId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "Stores");
        }
    }
}
