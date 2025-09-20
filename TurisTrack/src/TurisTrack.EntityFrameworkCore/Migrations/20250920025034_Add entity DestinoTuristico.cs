using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddentityDestinoTuristico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDestinosTuristicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroCalle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Calle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Localidad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DireccionFormateada = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Latitud = table.Column<double>(type: "float", nullable: false),
                    Longitud = table.Column<double>(type: "float", nullable: false),
                    TipoUbicacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDestinosTuristicos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDestinosTuristicos");
        }
    }
}
