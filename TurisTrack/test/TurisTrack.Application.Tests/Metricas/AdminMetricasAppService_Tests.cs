using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace TurisTrack.Metricas
{
    public class AdminMetricasAppService_Tests<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly AdminMetricasAppService _adminMetricasAppService;
        private readonly IRepository<ApiMetrica, Guid> _metricasRepository;

        public AdminMetricasAppService_Tests()
        {
            _adminMetricasAppService = GetRequiredService<AdminMetricasAppService>();
            _metricasRepository = GetRequiredService<IRepository<ApiMetrica, Guid>>();
        }

        [Fact]
        public async Task Debe_Calcular_Metricas_Correctamente()
        {
            // Arrange: Crear datos de prueba simulados
            await _metricasRepository.InsertAsync(new ApiMetrica("Test1", "", 100, true));
            await _metricasRepository.InsertAsync(new ApiMetrica("Test2", "", 200, false)); // Fallo

            // Act: Ejecutar el servicio
            var resultado = await _adminMetricasAppService.ObtenerMetricasUsoAsync();

            // Assert: Verificar cálculos
            resultado.TotalPeticiones.ShouldBe(2);
            resultado.PeticionesFallidas.ShouldBe(1);
            resultado.TiempoPromedioMs.ShouldBe(150); // (100+200)/2
            resultado.TasaErroresPorcentaje.ShouldBe(50);
        }
    }
}