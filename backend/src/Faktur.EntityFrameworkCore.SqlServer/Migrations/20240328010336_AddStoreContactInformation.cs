using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faktur.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreContactInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressCountry",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressFormatted",
                table: "Stores",
                type: "nvarchar(1279)",
                maxLength: 1279,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLocality",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressPostalCode",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressRegion",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressStreet",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressVerifiedBy",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AddressVerifiedOn",
                table: "Stores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailVerifiedBy",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedOn",
                table: "Stores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAddressVerified",
                table: "Stores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "Stores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPhoneVerified",
                table: "Stores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneCountryCode",
                table: "Stores",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneE164Formatted",
                table: "Stores",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneExtension",
                table: "Stores",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Stores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneVerifiedBy",
                table: "Stores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhoneVerifiedOn",
                table: "Stores",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "AddressVerifiedBy",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AddressVerifiedOn",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedBy",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedOn",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "IsAddressVerified",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "IsPhoneVerified",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PhoneCountryCode",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PhoneE164Formatted",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PhoneExtension",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PhoneVerifiedBy",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PhoneVerifiedOn",
                table: "Stores");
        }
    }
}
