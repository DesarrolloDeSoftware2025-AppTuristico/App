using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using TurisTrack.Experiencias.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace TurisTrack.ExperienciasDeViajes
{
    public class ExperienciaAppService : ApplicationService, IExperienciaAppService
    {
        private readonly IRepository<ExperienciaDeViaje, Guid> _experienciaRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;

        public ExperienciaAppService(
            IRepository<ExperienciaDeViaje, Guid> experienciaRepository,
            IRepository<DestinoTuristico, Guid> destinoRepository)
        {
            _experienciaRepository = experienciaRepository;
            _destinoRepository = destinoRepository;
        }

        // 4.1. Crear nueva experiencia
        [Authorize]
        public async Task<string> CrearExperienciaAsync(Guid destinoId, string comentario, DateTime fechaVisita)
        {
            var destino = await _destinoRepository.FindAsync(destinoId);
            if (destino == null)
            {
                throw new UserFriendlyException("El destino turístico no existe.");
            }

            // El CreatorId se llenará solo al insertar
            var nuevaExperiencia = new ExperienciaDeViaje(
                destinoId,
                comentario,
                fechaVisita
            );

            await _experienciaRepository.InsertAsync(nuevaExperiencia);

            return "Experiencia de Viaje creada correctamente";
        }

        // 4.2. Editar experiencia propia
        [Authorize]
        public async Task<string> EditarExperienciaAsync(Guid experienciaId, string? comentario = null, DateTime? fechaVisita = null)
        {
            var experiencia = await _experienciaRepository.FindAsync(experienciaId);
            if (experiencia == null)
            {
                throw new UserFriendlyException("La experiencia que busca no existe.");
            }

            // Verificamos usando el campo heredado CreatorId
            if (experiencia.CreatorId != CurrentUser.Id)
            {
                throw new UserFriendlyException("No tienes permiso para editar esta experiencia.");
            }

            if (!string.IsNullOrWhiteSpace(comentario))
            {
                experiencia.SetComentario(comentario);
            }

            if (fechaVisita.HasValue)
            {
                experiencia.FechaVisita = fechaVisita.Value;
            }

            await _experienciaRepository.UpdateAsync(experiencia);

            return "Experiencia de Viaje modificada correctamente";
        }

        // 4.3. Eliminar experiencia propia
        [Authorize]
        public async Task<string> EliminarExperienciaAsync(Guid experienciaId)
        {
            var experiencia = await _experienciaRepository.FindAsync(experienciaId);

            if (experiencia == null)
            {
                throw new UserFriendlyException("La experiencia que busca no existe.");
            }

            if (experiencia.CreatorId != CurrentUser.Id)
            {
                throw new UserFriendlyException("No tienes permiso para eliminar esta experiencia.");
            }

            await _experienciaRepository.DeleteAsync(experiencia);

            return "Experiencia de Viaje eliminada correctamente";
        }

        // 4.4. Consultar experiencias de OTROS usuarios en un destino
        public async Task<List<ExperienciaDeViajeDto>> ObtenerExperienciasDeOtrosAsync(Guid destinoId)
        {
            var experiencias = await _experienciaRepository.GetListAsync(x =>
                x.DestinoId == destinoId &&       // Filtro por destino
                x.CreatorId != CurrentUser.Id     // Filtro que no sean del usuario actual
            );

            return ObjectMapper.Map<List<ExperienciaDeViaje>, List<ExperienciaDeViajeDto>>(experiencias);
        }

        // 4.5. Filtrar experiencias por valoración (Sentimiento)
        public async Task<List<ExperienciaDeViajeDto>> FiltrarPorSentimientoAsync(Guid destinoId, SentimientoExperiencia sentimiento)
        {
            // Filtramos directamente en base de datos gracias a que guardamos el Enum
            var experienciasFiltradas = await _experienciaRepository.GetListAsync(x =>
                x.DestinoId == destinoId &&
                x.Sentimiento == sentimiento
            );

            return ObjectMapper.Map<List<ExperienciaDeViaje>, List<ExperienciaDeViajeDto>>(experienciasFiltradas);
        }

        // 4.6. Implementación de búsqueda Global por palabra clave
        public async Task<List<ExperienciaDeViajeDto>> BuscarPorPalabraClaveAsync(string palabraClave)
        {
            // Si la búsqueda está vacía, devolvemos una lista vacía para no traer toda la base de datos.
            if (string.IsNullOrWhiteSpace(palabraClave))
            {
                return new List<ExperienciaDeViajeDto>();
            }

            var terminoBusqueda = palabraClave.Trim();

            // Consulta al repositorio TODAS las experiencias de TODOS los destinos.
            var experienciasEncontradas = await _experienciaRepository.GetListAsync(x =>
                x.Comentario.Contains(terminoBusqueda)
            );

            return ObjectMapper.Map<List<ExperienciaDeViaje>, List<ExperienciaDeViajeDto>>(experienciasEncontradas);
        }
    }
}