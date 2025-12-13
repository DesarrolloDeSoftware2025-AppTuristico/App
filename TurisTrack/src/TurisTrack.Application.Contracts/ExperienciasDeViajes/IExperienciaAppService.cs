using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurisTrack.Experiencias.Dtos;
using Volo.Abp.Application.Services;

namespace TurisTrack.ExperienciasDeViajes
{
    public interface IExperienciaAppService : IApplicationService
    {
        // 4.1. Crear
        Task<string> CrearExperienciaAsync(Guid destinoId, string comentario, DateTime fechaVisita);

        // 4.2. Editar
        Task<string> EditarExperienciaAsync(Guid experienciaId, string? comentario = null, DateTime? fechaVisita = null);

        // 4.3. Eliminar
        Task<string> EliminarExperienciaAsync(Guid experienciaId);

        // 4.4. Consultar experiencias de OTROS usuarios
        Task<List<ExperienciaDeViajeDto>> ObtenerExperienciasDeOtrosAsync(Guid destinoId);

        // 4.5. Filtrar experiencias por valoración (Sentimiento)
        Task<List<ExperienciaDeViajeDto>> FiltrarPorSentimientoAsync(Guid destinoId, SentimientoExperiencia sentimiento);

        // 4.6. Buscar por palabra clave (Búsqueda Global)
        Task<List<ExperienciaDeViajeDto>> BuscarPorPalabraClaveAsync(string palabraClave);
    }
}