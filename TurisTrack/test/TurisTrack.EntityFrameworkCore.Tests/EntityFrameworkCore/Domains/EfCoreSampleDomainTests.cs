using TurisTrack.Samples;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Domains;

[Collection(TurisTrackTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<TurisTrackEntityFrameworkCoreTestModule>
{

}
