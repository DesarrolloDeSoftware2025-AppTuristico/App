using Shouldly;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace TurisTrack.Tests.DestinosTuristicos
{
    public class DestinoTuristicoAppService_Tests : TurisTrackApplicationTestBase<IAbpModule>
    {

        private readonly IDestinoTuristicoAppService _service;

        public DestinoTuristicoAppService_Tests()
        {
            _service = GetRequiredService<IDestinoTuristicoAppService>();
        }

        [Fact]
        public async Task GuardarDestinoAsync_Deberia_Guardar_Correctamente()
        {
            // Arrange
            var dto = new DestinoTuristicoDto
            {
                IdAPI = 100,
                Tipo = "Ciudad",
                Nombre = "Buenos Aires",
                Pais = "Argentina",
                Region = "América del Sur",
                MetrosDeElevacion = 25,
                Latitud = -34.6037,
                Longitud = -58.3816,
                Poblacion = 2890151,
                Eliminado = false
            };

            // Act
            var result = await _service.GuardarDestinoAsync(dto);

            // Assert
            result.Message.ShouldContain("guardado correctamente");
        }
    }
    /*
    [Fact]
    public async Task GuardarDestinoAsync_No_Deberia_Guardar_Si_Nombre_Es_Nulo()
    {
        // Arrange
        var dto = new DestinoTuristicoDto
        {
            IdAPI = 101,
            Nombre = null, // nombre inválido
            Pais = "Argentina"
        };

        // Act
        var result = await _service.GuardarDestinoAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("No se encontró", result.Message);
    }

    [Fact]
    public async Task GuardarDestinoAsync_No_Deberia_Guardar_Si_Ya_Existe()
    {
        // Arrange
        var dto = new DestinoTuristicoDto
        {
            IdAPI = 102,
            Nombre = "Córdoba",
            Pais = "Argentina"
        };



        // Act
        var result = await _service.GuardarDestinoAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("ya existe", result.Message);
        Assert.Equal(existente.Id, result.IdInterno);
    }*/
}

