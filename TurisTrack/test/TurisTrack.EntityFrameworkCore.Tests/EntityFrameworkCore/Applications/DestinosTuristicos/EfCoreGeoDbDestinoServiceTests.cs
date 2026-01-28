using TurisTrack.DestinosTuristicos;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications.DestinosTuristicos
{
    [Collection(TurisTrackTestConsts.CollectionDefinitionName)]
    public class EfCoreGeoDbDestinoServiceTests : GeoDbDestinoServiceIntegrationTests<TurisTrackEntityFrameworkCoreTestModule>
    {
    }
}
