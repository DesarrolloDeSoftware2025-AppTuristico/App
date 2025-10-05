using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace TurisTrack.Tests.DestinosTuristicos
{
    public abstract class DestinoTuristicoAppService_Tests<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly DestinoTuristicoAppService _service;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;

        protected DestinoTuristicoAppService_Tests()
        {
            _service = GetRequiredService<DestinoTuristicoAppService>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
        }

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
            await WithUnitOfWorkAsync(async () => {
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
    }
}

