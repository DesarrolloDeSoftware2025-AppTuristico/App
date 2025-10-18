using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurisTrack.APIExterna;
using TurisTrack.DestinosTuristicos;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace TurisTrack.Tests.DestinosTuristicos
{
    public abstract class DestinoTuristicoAppService_Tests<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        //Prueba de la funcionalidad de guardar un destino turistico
        private readonly DestinoTuristicoAppService _service;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;

        //Prueba de la funcionalidad de buscar un destino turistico por nombre. Pais, region, poblacionMinima (Opcionales)

        private readonly Mock<IRepository<DestinoTuristico, Guid>> _mockRepo;
        private readonly Mock<IGeoDbDestinoService> _mockGeoDbService;
        private readonly DestinoTuristicoAppService _appService;


        protected DestinoTuristicoAppService_Tests()
        {
            //Prueba de la funcionalidad de guardar un destino turistico
            _service = GetRequiredService<DestinoTuristicoAppService>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();

            //Prueba de la funcionalidad de buscar un destino turistico por nombre. Pais, region, poblacionMinima (Opcionales)

            _mockRepo = new Mock<IRepository<DestinoTuristico, Guid>>();
            _mockGeoDbService = new Mock<IGeoDbDestinoService>(); // no necesitamos HttpClient aquí
            _appService = new DestinoTuristicoAppService(_mockRepo.Object, _mockGeoDbService.Object);

        }

        //Prueba de la funcionalidad de guardar un destino turistico
        [Fact]
        public async Task GuardarDestinoAsync_Deberia_Guardar_Correctamente()
        {
            await WithUnitOfWorkAsync(async () =>
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
                result.ShouldNotBeNull();
                result.Message.ShouldContain("guardado correctamente");
            });
        }


        [Fact]
        public async Task GuardarDestinoAsync_No_Deberia_Guardar_Valores_Nulos()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                var dto = new DestinoTuristicoDto
                {
                    IdAPI = 101,
                    Tipo = "Ciudad",
                    Nombre = null,
                    Pais = "Argentina",
                    Region = null,
                    MetrosDeElevacion = 133,
                    Latitud = -45.6037,
                    Longitud = -57.3816,
                    Poblacion = 2894444,
                    Eliminado = false
                };

                // Act & Assert
                var exception = await Assert.ThrowsAsync<BusinessException>(
                    async () => await _service.GuardarDestinoAsync(dto));

                // Verificamos que el código de error o el mensaje contenga lo esperado
                exception.Code.ShouldBe("TurisTrack:CamposInvalidos");
                exception.Data["Message"].ToString().ShouldContain("campos obligatorios");
            });
        }

        [Fact]
        public async Task GuardarDestinoAsync_No_Deberia_Guardar_Si_Ya_Existe()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Insertar destino duplicado en la base de datos de test
                var existente = new DestinoTuristico
                {
                    IdAPI = 102,
                    Tipo = "Ciudad",
                    Nombre = "Akhtala",
                    Pais = "Armenia",
                    Region = "América del Sur",
                    MetrosDeElevacion = 25,
                    Latitud = -34.6037,
                    Longitud = -58.3816,
                    Poblacion = 2890151,
                    Eliminado = false
                };

                await _destinoRepository.InsertAsync(existente, autoSave: true);

                // Arrange
                var dto = new DestinoTuristicoDto
                {
                    IdAPI = 102,
                    Tipo = "Ciudad",
                    Nombre = "Akhtala",
                    Pais = "Armenia",
                    Region = "América del Sur",
                    MetrosDeElevacion = 25,
                    Latitud = -34.6037,
                    Longitud = -58.3816,
                    Poblacion = 2890151,
                    Eliminado = false
                };

                var exception = await Assert.ThrowsAsync<BusinessException>(
                async () => await _service.GuardarDestinoAsync(dto));

                // Verificamos que el código de error o el mensaje contenga lo esperado
                exception.Code.ShouldBe("TurisTrack:DestinoDuplicado");
                exception.Data["Message"].ToString().ShouldContain("ya existe");
            });

        }

        //Prueba de la funcionalidad de buscar un destino turistico por nombre. Pais, region, poblacionMinima (Opcionales)

        [Fact]
        public async Task BuscarDestinosAsync_Deberia_Devolver_Resultados()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                var mockData = new List<DestinoTuristicoDto>
            {
                new DestinoTuristicoDto { IdAPI = 1, Nombre = "Buenos Aires", Pais = "Argentina" },
                new DestinoTuristicoDto { IdAPI = 2, Nombre = "Barcelona", Pais = "España" }
            };

                _mockGeoDbService
                    .Setup(s => s.BuscarDestinosAsync("Bue", null, null, null))
                    .ReturnsAsync(mockData);

                // Act
                var result = await _appService.BuscarDestinosAsync("Bue");

                // Assert
                result.ShouldNotBeNull();
                result.Count.ShouldBe(2);
                result[0].Nombre.ShouldBe("Buenos Aires");
            });
        }

        [Fact]
        public async Task BuscarDestinosAsync_Deberia_Devolver_Lista_Vacia()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                _mockGeoDbService
                    .Setup(s => s.BuscarDestinosAsync("XYZ", null, null, null))
                    .ReturnsAsync(new List<DestinoTuristicoDto>());

                // Act
                var result = await _appService.BuscarDestinosAsync("XYZ");

                // Assert
                result.ShouldBeEmpty();
            });
        }

        [Fact]
        public async Task BuscarDestinosAsync_Deberia_Lanzar_Excepcion_Si_Nombre_Es_Nulo()
        {

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _appService.BuscarDestinosAsync(null);
            });

        }


        [Fact]
        public async Task BuscarDestinosAsync_Deberia_Manejar_Error_De_API()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                _mockGeoDbService
                    .Setup(s => s.BuscarDestinosAsync("Roma", null, null, null))
                    .ThrowsAsync(new Exception("Error en la API externa"));

                // Act & Assert
                var ex = await Should.ThrowAsync<Exception>(async () =>
                {
                    await _appService.BuscarDestinosAsync("Roma");
                });

                ex.Message.ShouldBe("Error en la API externa");

            });

        }
    }
}

