using TurisTrack.Samples;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications;

[Collection(TurisTrackTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<TurisTrackEntityFrameworkCoreTestModule>
{

}
