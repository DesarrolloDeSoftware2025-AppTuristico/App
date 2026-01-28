using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Collections.Generic;
using TurisTrack.APIExterna;
using TurisTrack.DestinosTuristicos;
using Volo.Abp.Modularity;

namespace TurisTrack;

[DependsOn(
    typeof(TurisTrackApplicationModule),
    typeof(TurisTrackDomainTestModule)
)]
public class TurisTrackApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        /*
        var services = context.Services;

        // 🔧 Reemplazamos GeoDbDestinoService por un mock (para que no intente usar HttpClient real)
        var geoDbMock = new Mock<IGeoDbDestinoService>();
        geoDbMock
            .Setup(s => s.BuscarDestinosAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(new List<DestinoTuristicoDto>());

        services.AddSingleton(geoDbMock.Object);
        */

        // Crear el mock del servicio externo
        var geoDbMock = new Mock<IGeoDbDestinoService>();

        // Configurar el mock para que devuelva valores predecibles
        geoDbMock
            .Setup(s => s.BuscarDestinosAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(new List<DestinoTuristicoDto>());

        geoDbMock
            .Setup(s => s.ObtenerDestinoPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new DestinoTuristicoDto
            {
                IdAPI = id,
                Nombre = "Destino de prueba",
                Pais = "Argentina"
            });

        // Reemplazar la implementación real por el mock
        context.Services.Replace(ServiceDescriptor.Singleton(geoDbMock.Object));
    }

}
