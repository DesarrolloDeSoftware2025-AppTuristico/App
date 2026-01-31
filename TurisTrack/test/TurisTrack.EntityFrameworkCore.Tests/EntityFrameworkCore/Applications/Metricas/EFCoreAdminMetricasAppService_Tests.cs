using TurisTrack.Metricas;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications.Metricas;

[Collection(TurisTrackTestConsts.CollectionDefinitionName)]

public class EFCoreAdminMetricasAppService_Tests : AdminMetricasAppService_Tests<TurisTrackEntityFrameworkCoreTestModule>
{
}
