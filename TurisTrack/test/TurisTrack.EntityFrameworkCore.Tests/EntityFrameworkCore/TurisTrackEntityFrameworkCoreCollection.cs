using Xunit;

namespace TurisTrack.EntityFrameworkCore;

[CollectionDefinition(TurisTrackTestConsts.CollectionDefinitionName)]
public class TurisTrackEntityFrameworkCoreCollection : ICollectionFixture<TurisTrackEntityFrameworkCoreFixture>
{

}
