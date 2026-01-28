using TurisTrack.Tests.DestinosTuristicos;
using Xunit;


namespace TurisTrack.EntityFrameworkCore.Applications.DestinosTuristicos;

[Collection(TurisTrackTestConsts.CollectionDefinitionName)]

public class EFDestinoTuristicoAppService_Tests : DestinoTuristicoAppService_Tests<TurisTrackEntityFrameworkCoreTestModule>
{
}

