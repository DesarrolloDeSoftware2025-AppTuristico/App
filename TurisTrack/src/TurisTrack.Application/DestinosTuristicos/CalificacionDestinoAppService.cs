using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace TurisTrack.DestinosTuristicos
{
    public class CalificacionDestinoAppService : ApplicationService
    {
        private readonly IRepository<CalificacionDestino, Guid> _calificacionRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly ICurrentUser _currentUser;

        public CalificacionDestinoAppService(
            IRepository<CalificacionDestino, Guid> calificacionRepository,
            IRepository<DestinoTuristico, Guid> destinoRepository,
            ICurrentUser currentUser)
        {
            _calificacionRepository = calificacionRepository;
            _destinoRepository = destinoRepository;
            _currentUser = currentUser;
        }

        [Authorize]
        public async Task<String> CrearCalificacionAsync(Guid destinoId, int puntuacion, string? comentario = null)
        {
            // Verificar que el usuario esté autenticado
            if (!_currentUser.IsAuthenticated)
            {
                throw new ApplicationException("El usuario no está autenticado.");
            }

            // Validar que la puntuación esté entre 1 y 5
            if (puntuacion < 1 || puntuacion > 5)
                throw new ApplicationException("La puntuación debe estar entre 1 y 5.");

            // Validar que el destino exista
            var destino = await _destinoRepository.FindAsync(destinoId);
            if (destino == null)
                throw new ApplicationException("Destino turístico no encontrado.");

            // Verificar que el usuario no haya calificado ya este destino
            var misCalificaciones = await ObtenerMisCalificacionesAsync();
            var yaCalificado = misCalificaciones.Any(c => c.DestinoTuristicoId == destinoId);

            if (yaCalificado)
                throw new ApplicationException("Ya has calificado este destino anteriormente.");

            var calificacion = new CalificacionDestino(
                _currentUser.GetId(),
                destinoId,
                puntuacion,
                comentario
            );

            await _calificacionRepository.InsertAsync(calificacion, autoSave: true);

            return "Calificación creada exitosamente.";
        }

        [Authorize]
        public async Task<List<CalificacionDestinoDto>> ObtenerMisCalificacionesAsync()
        {
            // El filtro IUserOwned ya se aplica automáticamente
            var calificaciones = await _calificacionRepository.GetListAsync();

            // Mapear a DTOs
            return ObjectMapper.Map<List<CalificacionDestino>, List<CalificacionDestinoDto>>(calificaciones);
        }

    }
}
