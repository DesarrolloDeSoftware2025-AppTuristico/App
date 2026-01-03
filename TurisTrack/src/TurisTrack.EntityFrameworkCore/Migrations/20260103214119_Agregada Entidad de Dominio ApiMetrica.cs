using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisTrack.Migrations
{
    /// <inheritdoc />
    public partial class AgregadaEntidaddeDominioApiMetrica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppApiMetricas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parametros = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuracionMs = table.Column<int>(type: "int", nullable: false),
                    FueExitoso = table.Column<bool>(type: "bit", nullable: false),
                    CodigoError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppApiMetricas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppApiMetricas");
        }
    }
}
