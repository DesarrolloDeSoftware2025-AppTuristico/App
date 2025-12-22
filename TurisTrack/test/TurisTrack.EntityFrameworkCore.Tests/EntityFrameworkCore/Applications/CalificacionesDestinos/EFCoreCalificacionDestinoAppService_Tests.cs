using TurisTrack.CalificacionesDestinos;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications.CalificacionesDestinos
{
    [Collection(TurisTrackTestConsts.CollectionDefinitionName)]
    public class EFCoreCalificacionDestinoAppService_Tests : CalificacionDestinoAppService_Tests<TurisTrackEntityFrameworkCoreTestModule>
    {
    }
}
