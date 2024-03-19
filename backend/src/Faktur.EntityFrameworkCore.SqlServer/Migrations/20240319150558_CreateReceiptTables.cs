using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateReceiptTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    ReceiptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ItemCount = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "money", nullable: false),
                    Total = table.Column<decimal>(type: "money", nullable: false),
                    HasBeenProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.ReceiptId);
                    table.ForeignKey(
                        name: "FK_Receipts_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptItems",
                columns: table => new
                {
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: false),
                    Gtin = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    GtinNormalized = table.Column<long>(type: "bigint", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    SkuNormalized = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Label = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Flags = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: true),
                    DepartmentNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptItems", x => new { x.ReceiptId, x.Number });
                    table.ForeignKey(
                        name: "FK_ReceiptItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ReceiptItems_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "ReceiptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptTaxes",
                columns: table => new
                {
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    TaxableAmount = table.Column<decimal>(type: "money", nullable: false),
                    Amount = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptTaxes", x => new { x.ReceiptId, x.Code });
                    table.ForeignKey(
                        name: "FK_ReceiptTaxes_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "ReceiptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_DepartmentName",
                table: "ReceiptItems",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_DepartmentNumber",
                table: "ReceiptItems",
                column: "DepartmentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_Flags",
                table: "ReceiptItems",
                column: "Flags");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_Gtin",
                table: "ReceiptItems",
                column: "Gtin");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_GtinNormalized",
                table: "ReceiptItems",
                column: "GtinNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_Label",
                table: "ReceiptItems",
                column: "Label");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_Number",
                table: "ReceiptItems",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_Price",
                table: "ReceiptItems",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_ProductId",
                table: "ReceiptItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_Quantity",
                table: "ReceiptItems",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_Sku",
                table: "ReceiptItems",
                column: "Sku");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_SkuNormalized",
                table: "ReceiptItems",
                column: "SkuNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_UnitPrice",
                table: "ReceiptItems",
                column: "UnitPrice");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_AggregateId",
                table: "Receipts",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_CreatedBy",
                table: "Receipts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_CreatedOn",
                table: "Receipts",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_HasBeenProcessed",
                table: "Receipts",
                column: "HasBeenProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_IssuedOn",
                table: "Receipts",
                column: "IssuedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ItemCount",
                table: "Receipts",
                column: "ItemCount");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Number",
                table: "Receipts",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ProcessedBy",
                table: "Receipts",
                column: "ProcessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ProcessedOn",
                table: "Receipts",
                column: "ProcessedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_StoreId",
                table: "Receipts",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_SubTotal",
                table: "Receipts",
                column: "SubTotal");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Total",
                table: "Receipts",
                column: "Total");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_UpdatedBy",
                table: "Receipts",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_UpdatedOn",
                table: "Receipts",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Version",
                table: "Receipts",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptTaxes_Amount",
                table: "ReceiptTaxes",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptTaxes_Code",
                table: "ReceiptTaxes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptTaxes_Rate",
                table: "ReceiptTaxes",
                column: "Rate");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptTaxes_TaxableAmount",
                table: "ReceiptTaxes",
                column: "TaxableAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptItems");

            migrationBuilder.DropTable(
                name: "ReceiptTaxes");

            migrationBuilder.DropTable(
                name: "Receipts");
        }
    }
}
