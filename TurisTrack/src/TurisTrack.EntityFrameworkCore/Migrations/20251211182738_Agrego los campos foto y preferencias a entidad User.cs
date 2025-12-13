using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisTrack.Migrations
{
    /// <inheritdoc />
    public partial class AgregoloscamposfotoypreferenciasaentidadUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "AbpUsers",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Preferencias",
                table: "AbpUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Preferencias",
                table: "AbpUsers");
        }
    }
}
