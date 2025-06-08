using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransportUnitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransportUnits",
                columns: table => new
                {
                    TransportUnitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LorryID = table.Column<int>(type: "int", nullable: false),
                    DriverID = table.Column<int>(type: "int", nullable: false),
                    AssistantID = table.Column<int>(type: "int", nullable: false),
                    Container = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportUnits", x => x.TransportUnitID);
                    table.ForeignKey(
                        name: "FK_TransportUnits_Assistants_AssistantID",
                        column: x => x.AssistantID,
                        principalTable: "Assistants",
                        principalColumn: "AssistantID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransportUnits_Drivers_DriverID",
                        column: x => x.DriverID,
                        principalTable: "Drivers",
                        principalColumn: "DriverID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransportUnits_Lorries_LorryID",
                        column: x => x.LorryID,
                        principalTable: "Lorries",
                        principalColumn: "LorryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransportUnits_AssistantID",
                table: "TransportUnits",
                column: "AssistantID");

            migrationBuilder.CreateIndex(
                name: "IX_TransportUnits_DriverID",
                table: "TransportUnits",
                column: "DriverID");

            migrationBuilder.CreateIndex(
                name: "IX_TransportUnits_LorryID",
                table: "TransportUnits",
                column: "LorryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransportUnits");
        }
    }
}
