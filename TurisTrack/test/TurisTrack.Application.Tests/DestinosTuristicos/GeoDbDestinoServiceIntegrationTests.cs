using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.APIExterna;
using Volo.Abp.Modularity;
using Xunit;

namespace TurisTrack.DestinosTuristicos
{
    public abstract class GeoDbDestinoServiceIntegrationTests<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {

        private readonly GeoDbDestinoService _geoDbService;


        protected GeoDbDestinoServiceIntegrationTests()
        {

            _geoDbService = GetRequiredService<GeoDbDestinoService>();

        }



        [Fact]
        public async Task BuscarDestinosAsync_DeberiaDevolverResultadosReales()
        {

            {
                await WithUnitOfWorkAsync(async () =>
                {
                    // Arrange
                    int id = 3341803;

                    // Act
                    var resultado = await _geoDbService.ObtenerDestinoPorIdAsync(id);

                    // Assert
                    resultado.IdAPI.ShouldBe(id);
                    resultado.Nombre.ShouldBe("Romang");
                    resultado.Pais.ShouldBe("Argentina");
                    resultado.Region.ShouldBe("Santa Fe Province");
                    resultado.Latitud.ShouldBe(-29.5);
                    resultado.Longitud.ShouldBe(-59.76666667);
                });

            }
        }

        [Fact]
        public async Task BuscarDestinosAsync_DeberiaManejarErrorDeRed()
        {
            // Arrange
            // Forzamos un HttpClient inválido
            var httpClientFactory = new HttpClientFactoryFalsa();
            var servicioErroneo = new GeoDbDestinoService(httpClientFactory);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await servicioErroneo.BuscarDestinosAsync("Paris");
            });
        }


        private class HttpClientFactoryFalsa : IHttpClientFactory
        {
            public HttpClient CreateClient(string name)
            {
                return new HttpClient
                {
                    BaseAddress = new Uri("https://url-invalida.com")
                };
            }
        }

        [Fact]

        public async Task BuscarDestinosAsync_DeberiaMapearDTOsCorrectamente()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                string nombre = "Romang";

                // Act
                var resultados = await _geoDbService.BuscarDestinosAsync(nombre);

                var dto = resultados.First();


                dto.IdAPI.ShouldBeGreaterThan(0);
                dto.Nombre.ShouldNotBeNull();
                dto.Pais.ShouldNotBeNull();
                dto.CodigoPais.ShouldNotBeNull();
                dto.Latitud.ShouldBeInRange(-90, 90);
                dto.Longitud.ShouldBeInRange(-180, 180);
                dto.Region.ShouldNotBeNull();
                dto.Tipo.ShouldNotBeNull();
                dto.CodigoRegion.ShouldNotBeNull();
                dto.MetrosDeElevacion.ShouldBeGreaterThanOrEqualTo(0);
                dto.Poblacion.ShouldBeGreaterThanOrEqualTo(0);
                dto.Eliminado.ShouldBeFalse();



            });
        }

    }
}
