using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace TurisTrack.DestinosTuristicos
{
    public abstract class CalificacionDestinoAppService_Tests<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly CalificacionDestinoAppService _serviceCalificaciones;
        private readonly DestinoTuristicoAppService _serviceDestinos;
        private readonly IRepository<CalificacionDestino, Guid> _creacionDestinoRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;


        protected CalificacionDestinoAppService_Tests()
        {
            //Prueba de la funcionalidad de guardar un destino turistico
            _serviceCalificaciones = GetRequiredService<CalificacionDestinoAppService>();
            _serviceDestinos = GetRequiredService<DestinoTuristicoAppService>();
            _creacionDestinoRepository = GetRequiredService<IRepository<CalificacionDestino, Guid>>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Destino_Guardado_Correctamente()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var existente = new DestinoTuristico(
                    102,
                    "Ciudad",
                    "Akhtala",
                    "Armenia",
                    "América del Sur",
                    "",
                    "",
                    25,
                    -34.6037,
                    -58.3816,
                    2890151
                );

                await _destinoRepository.InsertAsync(existente, autoSave: true);

                var destinos = await _serviceDestinos.ListarDestinosGuardadosAsync();

                var destinoId = destinos.First().Id;

                // Act
                var response = await _serviceCalificaciones.CrearCalificacionAsync(destinoId, 5, "Excelente destino turístico!");

                // Assert
                response.ShouldContain("creada exitosamente");
            });
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Destino_No_Encontrado()
        {
            await WithUnitOfWorkAsync(async () =>
            {

                // Act
                var exception = await Assert.ThrowsAsync<ApplicationException>(
                    async () => await _serviceCalificaciones.CrearCalificacionAsync(Guid.NewGuid(), 5, "Excelente destino turístico!"));

                // Assert
                exception.Message.ShouldContain("no encontrado");
            });
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Control_Rango_Puntuacion()
        {
            await WithUnitOfWorkAsync(async () =>
            {

                // Act
                var exception = await Assert.ThrowsAsync<ApplicationException>(
                    async () => await _serviceCalificaciones.CrearCalificacionAsync(Guid.NewGuid(), 7, "Excelente destino turístico!"));

                // Assert
                exception.Message.ShouldContain("entre 1 y 5");
            });
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Comentario_Opcional()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var existente = new DestinoTuristico(
                    102,
                    "Ciudad",
                    "Akhtala",
                    "Armenia",
                    "América del Sur",
                    "",
                    "",
                    25,
                    -34.6037,
                    -58.3816,
                    2890151
                );

                await _destinoRepository.InsertAsync(existente, autoSave: true);

                var destinos = await _serviceDestinos.ListarDestinosGuardadosAsync();

                var destinoId = destinos.First().Id;

                // Act
                var response = await _serviceCalificaciones.CrearCalificacionAsync(destinoId, 5);

                // Assert
                response.ShouldContain("creada exitosamente");
            });
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Comprobar_Duplicado()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var existente = new DestinoTuristico(
                    102,
                    "Ciudad",
                    "Akhtala",
                    "Armenia",
                    "América del Sur",
                    "",
                    "",
                    25,
                    -34.6037,
                    -58.3816,
                    2890151
                );

                await _destinoRepository.InsertAsync(existente, autoSave: true);

                var destinos = await _serviceDestinos.ListarDestinosGuardadosAsync();

                var destinoId = destinos.First().Id;

                var response = await _serviceCalificaciones.CrearCalificacionAsync(destinoId, 5, "Excelente destino turístico!");

                // Act
                var exception = await Assert.ThrowsAsync<ApplicationException>(
                    async () => await _serviceCalificaciones.CrearCalificacionAsync(destinoId, 5, "Excelente destino turístico!"));

                // Assert
                exception.Message.ShouldContain("calificado este destino anteriormente");
            });
        }
    }
}
