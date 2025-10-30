using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace TurisTrack.DestinosTuristicos
{
    public abstract class CalificacionDestinoAppService_IntegrationTests<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly CalificacionDestinoAppService _calificacionAppService;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly IRepository<CalificacionDestino, Guid> _calificacionRepository;
        private readonly ICurrentUser _currentUser;

        public CalificacionDestinoAppService_IntegrationTests()
        {
            _calificacionAppService = GetRequiredService<CalificacionDestinoAppService>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }
        /*
        [Fact]
        public async Task Deberia_Requerir_Autenticacion_Para_Calificar()
        {
            // Act & Assert
            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                using (_currentUser.IsAuthenticated) // Usuario no autenticado
                {
                    var destino = await CrearDestinoDePruebaAsync();

                    await _calificacionAppService.CrearCalificacionAsync(destino.Id, 4, "Muy bueno");
                }
            });
        }*/

        [Fact]
        public async Task Deberia_Respetar_Filtro_Por_Usuario()
        {

            // Arrange
            var destino1 = new DestinoTuristicoDto
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

            var destino2 = new DestinoTuristicoDto
            {
                IdAPI = 101,
                Tipo = "Ciudad",
                Nombre = "Rosario",
                Pais = "Argentina",
                Region = "América del Sur",
                MetrosDeElevacion = 25,
                Latitud = -34.6037,
                Longitud = -58.3816,
                Poblacion = 2890151,
                Eliminado = false
            };

            var destinoObj1 = await CrearDestinoDePruebaAsync(destino1);
            var destinoObj2 = await CrearDestinoDePruebaAsync(destino2);

            /*
            var calificacion = new CalificacionDestinoDto
            {
                UserId = new Guid("1D7BF19C-1111-1111-1111-3A1D0BA73CE2"),
                DestinoTuristicoId = destinoObj1.Id,
                Puntuacion = 2,
                Comentario = "Argentina",
            };*/

            //var respuesta1 = await CrearCalificacionAsync(calificacion);

            var mockCurrentUser = new Mock<ICurrentUser>();
            mockCurrentUser.Setup(x => x.Id).Returns(Guid.NewGuid());
            mockCurrentUser.Setup(x => x.Email).Returns("test@user.com");

            var respuesta1 = await _calificacionAppService.CrearCalificacionAsync(destinoObj1.Id, 5, "Excelente destino!");


            var mockCurrentUser2 = new Mock<ICurrentUser>();
            mockCurrentUser.Setup(x => x.Id).Returns(new Guid("1D7BF19C-1111-1111-1111-3A1D0BA73CE2"));
            mockCurrentUser.Setup(x => x.Email).Returns("test@user.com");

            //var respuesta2 = await _calificacionAppService.CrearCalificacionAsync(destinoObj1.Id, 5, "Excelente destino!");
            var respuesta3 = await _calificacionAppService.CrearCalificacionAsync(destinoObj2.Id, 5, "Excelente destino!");



            // Act: obtener calificaciones solo del current usuario 
            //var listaCalificaciones = await _calificacionAppService.ObtenerMisCalificacionesAsync();

            // Assert
            //listaCalificaciones.Count.ShouldBe(2);
            respuesta1.ShouldBe("Calificación creada exitosamente.");
            respuesta3.ShouldBe("Calificación creada exitosamente.");

        }

        private async Task<DestinoTuristico> CrearDestinoDePruebaAsync(DestinoTuristicoDto destinoDto)
        {
            var destino = new DestinoTuristico(
                destinoDto.IdAPI,
                destinoDto.Tipo,
                destinoDto.Nombre,
                destinoDto.Region,
                destinoDto.Pais
            );
            await _destinoRepository.InsertAsync(destino, autoSave: true);
            return destino;
        }

        // Metodo auxiliar
        public async Task<String> CrearCalificacionAsync(CalificacionDestinoDto calificacionDto)
        {
            var calificacion = new CalificacionDestino(
                calificacionDto.Id,
                calificacionDto.DestinoTuristicoId,
                calificacionDto.Puntuacion,
                calificacionDto.Comentario
            );

            await _calificacionRepository.InsertAsync(calificacion, autoSave: true);

            return "Calificación creada exitosamente.";
        }

    }
}

