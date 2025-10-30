﻿using Moq;
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

        public CalificacionDestinoAppService_IntegrationTests()
        {
            _calificacionAppService = GetRequiredService<CalificacionDestinoAppService>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
            _calificacionRepository = GetRequiredService<IRepository<CalificacionDestino, Guid>>();
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


            var respuesta1 =  await CrearCalificacionPorIdAsync(destinoObj1.Id, new Guid("1D7BF19C-1111-1111-201B-3A1D0BA73CE2"), 5, "Excelente destino!");


            var respuesta2 = await _calificacionAppService.CrearCalificacionAsync(destinoObj1.Id, 5, "Excelente destino!");
            var respuesta3 = await _calificacionAppService.CrearCalificacionAsync(destinoObj2.Id, 5, "Excelente destino!");



            // Act: obtener calificaciones solo del current usuario 
            var listaCalificaciones = await _calificacionAppService.ObtenerMisCalificacionesAsync();

            // Assert
            listaCalificaciones.Count.ShouldBe(2);
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
        private async Task<String> CrearCalificacionPorIdAsync(Guid destinoId, Guid userId, int puntuacion, string? comentario = null)
        {

            var calificacion = new CalificacionDestino(
                userId,
                destinoId,
                puntuacion,
                comentario
            );

            await _calificacionRepository.InsertAsync(calificacion, autoSave: true);

            return "Calificación creada exitosamente.";
        }

    }
}

