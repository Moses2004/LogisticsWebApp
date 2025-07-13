using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDateIntoInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceDate",
                table: "Invoices",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "Invoices");
        }
    }
}
