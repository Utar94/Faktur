using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Release_2_0_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ActorId);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    ArticleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gtin = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    GtinNormalized = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_Articles", x => x.ArticleId);
                });

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    BannerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_Banners", x => x.BannerId);
                });

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
                    DepartmentCount = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    NumberNormalized = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                    table.ForeignKey(
                        name: "FK_Departments_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    SkuNormalized = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flags = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: true),
                    UnitType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "ReceiptItems",
                columns: table => new
                {
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Gtin = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    GtinNormalized = table.Column<long>(type: "bigint", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    SkuNormalized = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Label = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Flags = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: false),
                    DepartmentNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Actors_DisplayName",
                table: "Actors",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_EmailAddress",
                table: "Actors",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_Id",
                table: "Actors",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Actors_IsDeleted",
                table: "Actors",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_Type",
                table: "Actors",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_AggregateId",
                table: "Articles",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CreatedBy",
                table: "Articles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CreatedOn",
                table: "Articles",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DisplayName",
                table: "Articles",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Gtin",
                table: "Articles",
                column: "Gtin");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_GtinNormalized",
                table: "Articles",
                column: "GtinNormalized",
                unique: true,
                filter: "[GtinNormalized] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UpdatedBy",
                table: "Articles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UpdatedOn",
                table: "Articles",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Version",
                table: "Articles",
                column: "Version");

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

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CreatedBy",
                table: "Departments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CreatedOn",
                table: "Departments",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DisplayName",
                table: "Departments",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Number",
                table: "Departments",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_StoreId_NumberNormalized",
                table: "Departments",
                columns: new[] { "StoreId", "NumberNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_UpdatedBy",
                table: "Departments",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_UpdatedOn",
                table: "Departments",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Products_AggregateId",
                table: "Products",
                column: "AggregateId",
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
                name: "IX_Products_StoreId_Sku",
                table: "Products",
                columns: new[] { "StoreId", "Sku" },
                unique: true,
                filter: "[Sku] IS NOT NULL");

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

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_CreatedBy",
                table: "ReceiptItems",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_CreatedOn",
                table: "ReceiptItems",
                column: "CreatedOn");

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
                name: "IX_ReceiptItems_UpdatedBy",
                table: "ReceiptItems",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptItems_UpdatedOn",
                table: "ReceiptItems",
                column: "UpdatedOn");

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
                name: "IX_Stores_DepartmentCount",
                table: "Stores",
                column: "DepartmentCount");

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
                name: "Actors");

            migrationBuilder.DropTable(
                name: "ReceiptItems");

            migrationBuilder.DropTable(
                name: "ReceiptTaxes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Banners");
        }
    }
}
