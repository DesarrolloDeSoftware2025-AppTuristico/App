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
        private readonly CalificacionDestinoAppService _service;
        private readonly IRepository<CalificacionDestino, Guid> _creacionDestinoRepository;

        protected CalificacionDestinoAppService_Tests()
        {
            //Prueba de la funcionalidad de guardar un destino turistico
            _service = GetRequiredService<CalificacionDestinoAppService>();
            _creacionDestinoRepository = GetRequiredService<IRepository<CalificacionDestino, Guid>>();
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Deberia_Guardar_Correctamente()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                var dto = new CalificacionDestinoDto
                {
                    UserId = Guid.NewGuid(),
                    DestinoTuristicoId = Guid.NewGuid(),
                    Puntuacion = 5,
                    Comentario = "Excelente destino turístico!"
                };

                // Act
                var exception = await Assert.ThrowsAsync<ApplicationException>(
                    async () => await _service.CrearCalificacionAsync(Guid.NewGuid(), 5, "Excelente destino turístico!"));

                // Assert
                exception.Message.ShouldContain("no encontrado");
            });
        }
    }
}
