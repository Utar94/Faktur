using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptItemCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ReceiptItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_Category",
                table: "ReceiptItems",
                column: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReceiptItems_Category",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ReceiptItems");
        }
    }
}
