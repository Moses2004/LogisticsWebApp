using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderlineID = table.Column<int>(type: "int", nullable: false),
                    TransportUnitID = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceID);
                    table.ForeignKey(
                        name: "FK_Invoices_Orderlines_OrderlineID",
                        column: x => x.OrderlineID,
                        principalTable: "Orderlines",
                        principalColumn: "OrderlineID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_TransportUnits_TransportUnitID",
                        column: x => x.TransportUnitID,
                        principalTable: "TransportUnits",
                        principalColumn: "TransportUnitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OrderlineID",
                table: "Invoices",
                column: "OrderlineID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TransportUnitID",
                table: "Invoices",
                column: "TransportUnitID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");
        }
    }
}
