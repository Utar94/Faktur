using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faktur.Infrastructure.Migrations
{
    public partial class AddedProcessedReceipts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "Receipts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "Receipts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "Receipts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Processed",
                table: "Receipts",
                column: "Processed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Receipts_Processed",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Processed",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "Receipts");
        }
    }
}
