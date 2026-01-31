using TurisTrack.ExperienciasDeViajes;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications.ExperienciasDeViajes;

[Collection(TurisTrackTestConsts.CollectionDefinitionName)]

public class EFCoreExperienciaAppService_UnitTest : ExperienciaAppService_UnitTest<TurisTrackEntityFrameworkCoreTestModule>
{
}
