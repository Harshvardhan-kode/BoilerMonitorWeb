using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoilerMonitorWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "AlarmDefinitions",
                schema: "dbo",
                columns: table => new
                {
                    AlarmDefID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MessageTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmDefinitions", x => x.AlarmDefID);
                });

            migrationBuilder.CreateTable(
                name: "AlarmLogs",
                schema: "dbo",
                columns: table => new
                {
                    AlarmID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoilerID = table.Column<int>(type: "int", nullable: false),
                    AlarmDefID = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Acknowledged = table.Column<bool>(type: "bit", nullable: false),
                    AckBy = table.Column<int>(type: "int", nullable: true),
                    AckTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmLogs", x => x.AlarmID);
                });

            migrationBuilder.CreateTable(
                name: "BoilerLogs",
                schema: "dbo",
                columns: table => new
                {
                    BoilerID = table.Column<int>(type: "int", nullable: false),
                    LogTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SteamPressure_Bar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SteamTemp_C = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WaterLevel_mm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeedwaterTemp_C = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FlueGasTemp_C = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    O2_Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SteamFlow_KGHR = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Boilers",
                schema: "dbo",
                columns: table => new
                {
                    BoilerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Capacity_KGHR = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boilers", x => x.BoilerID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlarmDefinitions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AlarmLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BoilerLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Boilers",
                schema: "dbo");
        }
    }
}
