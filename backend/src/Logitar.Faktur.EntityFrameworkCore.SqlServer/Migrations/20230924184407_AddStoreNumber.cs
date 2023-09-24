using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Stores",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberNormalized",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Number",
                table: "Stores",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_NumberNormalized",
                table: "Stores",
                column: "NumberNormalized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_Number",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_NumberNormalized",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "NumberNormalized",
                table: "Stores");
        }
    }
}
