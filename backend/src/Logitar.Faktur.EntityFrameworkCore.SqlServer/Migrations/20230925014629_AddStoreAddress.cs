using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneE164Formatted",
                table: "Stores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressCountry",
                table: "Stores",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressFormatted",
                table: "Stores",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLocality",
                table: "Stores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressPostalCode",
                table: "Stores",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressRegion",
                table: "Stores",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressStreet",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_AddressFormatted",
                table: "Stores",
                column: "AddressFormatted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_AddressFormatted",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AddressCountry",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AddressFormatted",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AddressLocality",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AddressPostalCode",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AddressRegion",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AddressStreet",
                table: "Stores");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneE164Formatted",
                table: "Stores",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
