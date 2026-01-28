using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisTrack.Migrations
{
    /// <inheritdoc />
    public partial class ModificotipoGuidporintdelatributoIdAPIentidadDestinoTuristico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Eliminar la columna existente
            migrationBuilder.DropColumn(
                name: "IdAPI",
                table: "AppDestinosTuristicos");

            // 2. Agregar la nueva columna como int
            migrationBuilder.AddColumn<int>(
                name: "IdAPI",
                table: "AppDestinosTuristicos",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Primero eliminar la columna int que agregaste en el Up
            migrationBuilder.DropColumn(
                name: "IdAPI",
                table: "AppDestinosTuristicos");

            // 2. Luego agregar de nuevo la columna Guid como estaba originalmente
            migrationBuilder.AddColumn<Guid>(
                name: "IdAPI",
                table: "AppDestinosTuristicos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000")); // Puedes usar Guid.Empty
        }
    }
}
