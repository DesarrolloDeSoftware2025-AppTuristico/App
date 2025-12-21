using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace TurisTrack.CalificacionesDestinos
{
    public class CalificacionDestinoAppService : ApplicationService, ICalificacionDestinoAppService
    {
        private readonly IRepository<CalificacionDestino, Guid> _calificacionRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IDataFilter _dataFilter;

        public CalificacionDestinoAppService(
            IRepository<CalificacionDestino, Guid> calificacionRepository,
            IRepository<DestinoTuristico, Guid> destinoRepository,
            ICurrentUser currentUser,
            IIdentityUserRepository userRepository,
            IDataFilter dataFilter)
        {
            _calificacionRepository = calificacionRepository;
            _destinoRepository = destinoRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _dataFilter = dataFilter;
        }

        [Authorize]
        public async Task<string> CrearCalificacionAsync(Guid destinoId, int puntuacion, string? comentario = null)
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
            {
                throw new UserFriendlyException("Ya has calificado este destino anteriormente.");
            }
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

        // 5.3. Editar calificación propia
        [Authorize]
        public async Task<String> EditarCalificacionAsync(Guid calificacionId, int? nuevaPuntuacion = null, string? nuevoComentario = null)
        {
            // 1. Obtener la calificación
            var calificacion = await _calificacionRepository.FindAsync(calificacionId);
            if (calificacion == null)
            {
                throw new UserFriendlyException("La calificación que busca no existe.");
            }

            // 2. Verificar que el usuario actual sea el dueño
            if (calificacion.UserId != _currentUser.GetId())
            {
                throw new UserFriendlyException("No tienes permisos para editar esta calificación.");
            }

            // 3. Actualizar campos
            if (nuevaPuntuacion.HasValue)
            {
                if (nuevaPuntuacion.Value < 1 || nuevaPuntuacion.Value > 5)
                {
                    throw new UserFriendlyException("La puntuación debe estar entre 1 y 5.");
                }

                calificacion.Puntuacion = nuevaPuntuacion.Value;
            }

            if (!string.IsNullOrWhiteSpace(nuevoComentario))
            {
                calificacion.Comentario = nuevoComentario;
            }

            // 4. Guardar cambios
            await _calificacionRepository.UpdateAsync(calificacion);

            return "Calificación modificada correctamente";
        }

        // 5.3. Eliminar calificación propia
        [Authorize]
        public async Task<String> EliminarCalificacionAsync(Guid calificacionId)
        {
            var calificacion = await _calificacionRepository.GetAsync(calificacionId);

            if (calificacion.UserId != _currentUser.GetId())
            {
                throw new UserFriendlyException("No tienes permisos para eliminar esta calificación.");
            }

            await _calificacionRepository.DeleteAsync(calificacion);

            return "Calificación eliminada correctamente";
        }

        // 5.4. Consultar promedio de calificaciones de un destino
        [AllowAnonymous]
        public async Task<ResumenCalificacionDto> ObtenerPromedioDestinoAsync(Guid destinoId)
        {
            using (_dataFilter.Disable<IUserOwned>())
            {
                // Usamos GetQueryableAsync para hacer consultas LINQ eficientes a la BD
                var query = await _calificacionRepository.GetQueryableAsync();

                var calificacionesDestino = query.Where(c => c.DestinoTuristicoId == destinoId);

                // Si no hay calificaciones, retornamos 0
                if (!calificacionesDestino.Any())
                {
                    return new ResumenCalificacionDto { Promedio = 0, TotalCalificaciones = 0 };
                }

                var promedio = calificacionesDestino.Average(c => c.Puntuacion);
                var total = calificacionesDestino.Count();

                return new ResumenCalificacionDto
                {
                    Promedio = Math.Round(promedio, 1), // Solo con 1 decimal
                    TotalCalificaciones = total
                };
            }
        }

        // 5.5. Listar comentarios de un destino
        [AllowAnonymous]
        public async Task<List<CalificacionDestinoDto>> ObtenerComentariosPorDestinoAsync(Guid destinoId)
        {
            using (_dataFilter.Disable<IUserOwned>())
            {
                // 1. Obtener las calificaciones del destino
                var query = await _calificacionRepository.GetQueryableAsync();

                var calificaciones = query
                    .Where(c => c.DestinoTuristicoId == destinoId)
                    .OrderByDescending(c => c.CreationTime) // Ordenar por fecha (más nuevo arriba)
                    .ToList();

                if (!calificaciones.Any())
                {
                    return new List<CalificacionDestinoDto>();
                }

                // 2. Mapear Entidad -> DTO (Esto llenará UserId, Puntuacion, Comentario, CreationTime)
                var dtos = ObjectMapper.Map<List<CalificacionDestino>, List<CalificacionDestinoDto>>(calificaciones);

                // 3. Obtener los IDs de usuarios únicos para no consultar a la BD 100 veces
                var userIds = dtos.Select(d => d.UserId).Distinct().ToArray();

                // 4. Consultar el repositorio de IdentityUsers para obtener los nombres
                // GetListByIdsAsync es un método optimizado de ABP Identity
                var users = await _userRepository.GetListByIdsAsync(userIds);

                // 5. Crear un diccionario para búsqueda rápida (Id -> UserName)
                var userDictionary = users.ToDictionary(u => u.Id, u => u.UserName);

                // 6. Rellenar el UserName en cada DTO
                foreach (var dto in dtos)
                {
                    if (userDictionary.ContainsKey(dto.UserId))
                    {
                        dto.UserName = userDictionary[dto.UserId];
                    }
                    else
                    {
                        dto.UserName = "Usuario eliminado"; // Por si el usuario ya no existe
                    }
                }
                return dtos;
            }
        }

    }
}
