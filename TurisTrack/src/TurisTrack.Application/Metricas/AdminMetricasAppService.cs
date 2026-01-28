using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TurisTrack.Metricas
{
    [Authorize(Roles = "admin")] 
    public class AdminMetricasAppService : ApplicationService
    {
        private readonly IRepository<ApiMetrica, Guid> _metricasRepository;

        public AdminMetricasAppService(IRepository<ApiMetrica, Guid> metricasRepository)
        {
            _metricasRepository = metricasRepository;
        }

        public async Task<ApiMetricaResumenDto> ObtenerMetricasUsoAsync()
        {
            var todasLasMetricas = await _metricasRepository.GetListAsync();

            if (!todasLasMetricas.Any()) return new ApiMetricaResumenDto();

            var resumen = new ApiMetricaResumenDto
            {
                TotalPeticiones = todasLasMetricas.Count,
                PeticionesExitosas = todasLasMetricas.Count(x => x.FueExitoso),
                PeticionesFallidas = todasLasMetricas.Count(x => !x.FueExitoso),
                TiempoPromedioMs = todasLasMetricas.Average(x => x.DuracionMs),
                EndpointMasLento = todasLasMetricas.OrderByDescending(x => x.DuracionMs).FirstOrDefault()?.Endpoint ?? "-"
            };

            // Cálculo de tasa de error
            resumen.TasaErroresPorcentaje = (double)resumen.PeticionesFallidas / resumen.TotalPeticiones * 100;

            return resumen;
        }
    }
}