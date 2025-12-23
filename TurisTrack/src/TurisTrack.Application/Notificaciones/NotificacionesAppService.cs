using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos; // Necesario para acceder a la entidad DestinoTuristico
using TurisTrack.DestinosTuristicos.Dtos;
using TurisTrack.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Identity;

namespace TurisTrack.Notificaciones
{
    [Authorize]
    public class NotificacionesAppService : ApplicationService, INotificacionAppService
    {
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly IIdentityUserRepository _identityUserRepository;

        public NotificacionesAppService(
            IRepository<Notificacion, Guid> notificacionRepository,
            IRepository<DestinoTuristico, Guid> destinoRepository,
            IIdentityUserRepository identityUserRepository)
        {
            _notificacionRepository = notificacionRepository;
            _destinoRepository = destinoRepository;
            _identityUserRepository = identityUserRepository;
        }

        // 7.2. Generar la notificación (Solo Admin)
        [Authorize(Roles = "Admin")]
        public async Task<String> ReportarEventoEnDestinoAsync(CrearEventoDestinoDto input)
        {
            // 1. Validar que el destino exista
            var destino = await _destinoRepository.FindAsync(input.DestinoId);
            if (destino == null)
            {
                throw new UserFriendlyException("El destino turístico no existe.");
            }

            // 2. Obtenemos TODOS los usuarios registrados
            var usuarios = await _identityUserRepository.GetListAsync();

            // 3. Preparamos la lista de notificaciones
            var notificacionesParaInsertar = new List<Notificacion>();

            foreach (var usuario in usuarios)
            {
                var nuevaNotificacion = new Notificacion(
                    usuario.Id,
                    destino.Id,
                    input.Titulo,
                    input.Mensaje,
                    input.Tipo
                );

                notificacionesParaInsertar.Add(nuevaNotificacion);
            }

            // 4. Inserción masiva (Mucho más rápido que insertar una por una)
            await _notificacionRepository.InsertManyAsync(notificacionesParaInsertar);

            return "Notificaciones generadas exitosamente para todos los usuarios.";
        }

        // 7.4. Marcar como leída
        public async Task MarcarComoLeidaAsync(Guid idNotificacion)
        {
            var notificacion = await _notificacionRepository.GetAsync(idNotificacion);

            // Validar que la notificación pertenece al usuario actual
            if (notificacion.UserId != CurrentUser.Id)
            {
                throw new UnauthorizedAccessException("No puedes modificar notificaciones de otros usuarios.");
            }

            notificacion.MarcarComoLeido();
            await _notificacionRepository.UpdateAsync(notificacion);
        }

        // 7.4. Marcar como no leída
        public async Task MarcarComoNoLeidaAsync(Guid idNotificacion)
        {
            var notificacion = await _notificacionRepository.GetAsync(idNotificacion);

            // Validar que la notificación pertenece al usuario actual
            if (notificacion.UserId != CurrentUser.Id)
            {
                throw new UnauthorizedAccessException("No puedes modificar notificaciones de otros usuarios.");
            }

            notificacion.MarcarComoNoLeido();
            await _notificacionRepository.UpdateAsync(notificacion);
        }

        // Listar mis notificaciones
        public async Task<List<NotificacionDto>> ObtenerMisNotificacionesAsync()
        {
            if (CurrentUser.Id == null) return new List<NotificacionDto>();

            // 1. Obtener las notificaciones del usuario
            var notificaciones = await _notificacionRepository.GetListAsync(n => n.UserId == CurrentUser.Id);

            // Ordenamos en memoria
            notificaciones = notificaciones.OrderByDescending(x => x.CreationTime).ToList();

            // 2. Usar AutoMapper para convertir la lista de Entidades a DTOs
            // Esto copia Id, Titulo, Mensaje, Tipo, Leido, CreationTime automáticamente.
            var dtos = ObjectMapper.Map<List<Notificacion>, List<NotificacionDto>>(notificaciones);

            // 3. Obtener los nombres de los destinos necesarios
            var destinoIds = notificaciones.Select(n => n.DestinoTuristicoId).Distinct().ToArray();

            // Hacemos UNA SOLA consulta para traer todos los destinos involucrados
            var destinos = await _destinoRepository.GetListAsync(d => destinoIds.Contains(d.Id));

            // Convertimos a diccionario para búsqueda instantánea en memoria
            var destinosDict = destinos.ToDictionary(d => d.Id, d => d.Nombre);

            // 4. Colocar el nombre del destino en el DTO correspondiente
            foreach (var dto in dtos)
            {
                if (destinosDict.TryGetValue(dto.DestinoTuristicoId, out var nombreDestino))
                {
                    dto.NombreDestino = nombreDestino;
                }
                else
                {
                    dto.NombreDestino = "Destino no disponible"; // Manejo de error si se borró el destino
                }
            }

            return dtos;
        }
    }
}