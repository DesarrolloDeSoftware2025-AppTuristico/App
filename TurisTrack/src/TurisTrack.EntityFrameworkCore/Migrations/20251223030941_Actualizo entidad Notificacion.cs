using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisTrack.Migrations
{
    /// <inheritdoc />
    public partial class ActualizoentidadNotificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinoRelacionadoId",
                table: "AppNotificaciones");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "AppNotificaciones",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AppNotificaciones_UsuarioId",
                table: "AppNotificaciones",
                newName: "IX_AppNotificaciones_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "AppNotificaciones",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinoTuristicoId",
                table: "AppNotificaciones",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinoTuristicoId",
                table: "AppNotificaciones");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AppNotificaciones",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_AppNotificaciones_UserId",
                table: "AppNotificaciones",
                newName: "IX_AppNotificaciones_UsuarioId");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "AppNotificaciones",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinoRelacionadoId",
                table: "AppNotificaciones",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
