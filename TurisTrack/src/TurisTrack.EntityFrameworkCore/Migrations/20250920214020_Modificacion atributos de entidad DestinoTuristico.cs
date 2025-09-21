using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisTrack.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionatributosdeentidadDestinoTuristico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calle",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "CodigoPostal",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "DireccionFormateada",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "Localidad",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "NumeroCalle",
                table: "AppDestinosTuristicos");

            migrationBuilder.RenameColumn(
                name: "TipoUbicacion",
                table: "AppDestinosTuristicos",
                newName: "Nombre");

            migrationBuilder.AddColumn<string>(
                name: "CodigoPais",
                table: "AppDestinosTuristicos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoRegion",
                table: "AppDestinosTuristicos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "AppDestinosTuristicos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "MetrosDeElevacion",
                table: "AppDestinosTuristicos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Poblacion",
                table: "AppDestinosTuristicos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "AppDestinosTuristicos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "AppDestinosTuristicos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZonaHoraria",
                table: "AppDestinosTuristicos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoPais",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "CodigoRegion",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "MetrosDeElevacion",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "Poblacion",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "AppDestinosTuristicos");

            migrationBuilder.DropColumn(
                name: "ZonaHoraria",
                table: "AppDestinosTuristicos");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "AppDestinosTuristicos",
                newName: "TipoUbicacion");

            migrationBuilder.AddColumn<string>(
                name: "Calle",
                table: "AppDestinosTuristicos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoPostal",
                table: "AppDestinosTuristicos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DireccionFormateada",
                table: "AppDestinosTuristicos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "AppDestinosTuristicos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Localidad",
                table: "AppDestinosTuristicos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumeroCalle",
                table: "AppDestinosTuristicos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
