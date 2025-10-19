using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
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
        public async Task CrearCalificacionAsync(Guid destinoId, int puntuacion, string? comentario)
        {
            if (_currentUser.Id == null)
                throw new ApplicationException("Debe estar autenticado para calificar un destino.");

            // Validar que el destino exista
            var destino = await _destinoRepository.FindAsync(destinoId);
            if (destino == null)
                throw new ApplicationException("Destino turístico no encontrado.");

            var calificacion = new CalificacionDestino(
                _currentUser.GetId(),
                destinoId,
                puntuacion,
                comentario
            );

            await _calificacionRepository.InsertAsync(calificacion, autoSave: true);
        }
    }
}
