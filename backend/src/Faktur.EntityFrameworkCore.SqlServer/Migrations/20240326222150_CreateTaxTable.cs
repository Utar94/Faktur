using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateTaxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Taxes",
                columns: table => new
                {
                    TaxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CodeNormalized = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    Flags = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxes", x => x.TaxId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_AggregateId",
                table: "Taxes",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_Code",
                table: "Taxes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_CodeNormalized",
                table: "Taxes",
                column: "CodeNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_CreatedBy",
                table: "Taxes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_CreatedOn",
                table: "Taxes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_Flags",
                table: "Taxes",
                column: "Flags");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_Rate",
                table: "Taxes",
                column: "Rate");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_UpdatedBy",
                table: "Taxes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_UpdatedOn",
                table: "Taxes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_Version",
                table: "Taxes",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Taxes");
        }
    }
}
