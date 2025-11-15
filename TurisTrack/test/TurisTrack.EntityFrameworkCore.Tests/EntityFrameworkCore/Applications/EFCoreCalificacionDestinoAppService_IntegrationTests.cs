using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications
{
    [Collection(TurisTrackTestConsts.CollectionDefinitionName)]

    public class EFCoreCalificacionDestinoAppService_IntegrationTests : CalificacionDestinoAppService_IntegrationTests<TurisTrackEntityFrameworkCoreTestModule>
    {
    }
}
