using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisTrack.Migrations
{
    /// <inheritdoc />
    public partial class CambiodetipoenentidadCalificacionDestino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppCalificacionesDestinos");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppCalificacionesDestinos");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppCalificacionesDestinos");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AppCalificacionesDestinos");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AppCalificacionesDestinos");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "AppCalificacionesDestinos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppCalificacionesDestinos");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppCalificacionesDestinos");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppCalificacionesDestinos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppCalificacionesDestinos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppCalificacionesDestinos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppCalificacionesDestinos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AppCalificacionesDestinos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AppCalificacionesDestinos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "AppCalificacionesDestinos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppCalificacionesDestinos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppCalificacionesDestinos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppCalificacionesDestinos",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
