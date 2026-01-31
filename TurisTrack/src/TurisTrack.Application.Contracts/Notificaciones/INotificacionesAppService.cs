using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TurisTrack.Notificaciones
{
    public interface INotificacionesAppService : IApplicationService
    {
        // 7.2 Notificar sobre cambios relevantes (usado por admins)
        Task<String> ReportarEventoEnDestinoAsync(CrearEventoDestinoDto input);

        // 7.4 Marcar como leída
        Task MarcarComoLeidaAsync(Guid id);
        // 7.4. Marcar como no leída
        Task MarcarComoNoLeidaAsync(Guid idNotificacion);

        // Obtener las notificaciones del usuario logueado
        Task<List<NotificacionDto>> ObtenerMisNotificacionesAsync();
    }
}