using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using TurisTrack.Tests.DestinosTuristicos;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications.DestinosTuristicos
{
    [Collection(TurisTrackTestConsts.CollectionDefinitionName)]
    public class EfCoreGeoDbDestinoServiceTests : GeoDbDestinoServiceIntegrationTests<TurisTrackEntityFrameworkCoreTestModule>
    {
    }
}
