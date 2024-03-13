using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateStoreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BannerId = table.Column<int>(type: "int", nullable: true),
                    Number = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreId);
                    table.ForeignKey(
                        name: "FK_Stores_Banners_BannerId",
                        column: x => x.BannerId,
                        principalTable: "Banners",
                        principalColumn: "BannerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_AggregateId",
                table: "Stores",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BannerId",
                table: "Stores",
                column: "BannerId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CreatedBy",
                table: "Stores",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CreatedOn",
                table: "Stores",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_DisplayName",
                table: "Stores",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Number",
                table: "Stores",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_UpdatedBy",
                table: "Stores",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_UpdatedOn",
                table: "Stores",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Version",
                table: "Stores",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
