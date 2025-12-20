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
                    string nombre = "Roma";

                    // Act
                    var resultados = await _geoDbService.BuscarDestinosAsync(nombre);

                    var dto = resultados.First();

                    // Assert
                    dto.IdAPI.ShouldBe(3341803);
                    dto.Nombre.ShouldBe("Romang");
                    dto.Pais.ShouldBe("Argentina");
                    dto.Region.ShouldBe("Santa Fe Province");
                    dto.Latitud.ShouldBe(-29.5);
                    dto.Longitud.ShouldBe(-59.76666667);
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

        [Fact]
        public async Task ObtenerDestinoPorIdAsync_Deberia_Devolver_Un_Destino()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                // Le pasamos el id 2 ya que es un destino conocido en la base de datos de GeoDB
                int id = 2;

                // Act
                var resultado = await _geoDbService.ObtenerDestinoPorIdAsync(id);

                // Assert
                resultado.IdAPI.ShouldBe(2);
                resultado.Nombre.ShouldBe("Akhtala");
                resultado.Pais.ShouldBe("Armenia");
                resultado.Region.ShouldBe("Lori Province");
                resultado.Latitud.ShouldBe(41,14952);
                resultado.Longitud.ShouldBe(44,78168);
            });
        }

    }
}
