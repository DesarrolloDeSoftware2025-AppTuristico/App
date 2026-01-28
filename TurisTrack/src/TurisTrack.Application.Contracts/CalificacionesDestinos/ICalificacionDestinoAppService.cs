using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TurisTrack.CalificacionesDestinos
{
    public interface ICalificacionDestinoAppService : IApplicationService
    {
        Task<string> CrearCalificacionAsync(Guid destinoId, int puntuacion, string? comentario = null);
        Task<List<CalificacionDestinoDto>> ObtenerMisCalificacionesAsync();
        Task<String> EditarCalificacionAsync(Guid calificacionId, int? nuevaPuntuacion = null, string? nuevoComentario = null);
        Task<String> EliminarCalificacionAsync(Guid calificacionId);
        Task<ResumenCalificacionDto> ObtenerPromedioDestinoAsync(Guid destinoId);
        Task<List<CalificacionDestinoDto>> ObtenerComentariosPorDestinoAsync(Guid destinoId);
    }
}
