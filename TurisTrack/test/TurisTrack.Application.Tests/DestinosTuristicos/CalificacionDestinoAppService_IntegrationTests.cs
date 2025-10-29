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
        private readonly ICurrentUser _currentUser;

        public CalificacionDestinoAppService_IntegrationTests()
        {
            _calificacionAppService = GetRequiredService<CalificacionDestinoAppService>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

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
        }

        [Fact]
        public async Task Deberia_Respetar_Filtro_Por_Usuario()
        {
            // Arrange
            var destino1 = await CrearDestinoDePruebaAsync();
            var destino2 = await CrearDestinoDePruebaAsync();

            // Usuario 1
            var user1Id = Guid.NewGuid();
            using (CurrentUser.Change(user1Id, "user1@test.com"))
            {
                await _calificacionAppService.CrearCalificacionAsync(destino1.Id, 5, "Excelente destino!");
            }

            // Usuario 2
            var user2Id = Guid.NewGuid();
            using (CurrentUser.Change(user2Id, "user2@test.com"))
            {
                await _calificacionAppService.CrearCalificacionAsync(destino2.Id, 3, "Regular");
            }

            // Act: obtener calificaciones solo del usuario 1
            using (CurrentUser.Change(user1Id, "user1@test.com"))
            {
                var misCalificaciones = await _calificacionAppService.ObtenerMisCalificacionesAsync();

                // Assert
                misCalificaciones.Count.ShouldBe(1);
                misCalificaciones.First().Comentario.ShouldBe("Excelente destino!");
            }
        }

        private async Task<DestinoTuristico> CrearDestinoDePruebaAsync()
        {
            var destino = new DestinoTuristico(Guid.NewGuid(), "Prueba", "Ciudad Test", "Pais Test");
            await _destinoRepository.InsertAsync(destino, autoSave: true);
            return destino;
        }
    }
}
}
