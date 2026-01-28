using TurisTrack.DestinosFavoritos;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications.DestinosFavoritos;

[Collection(TurisTrackTestConsts.CollectionDefinitionName)]

public class EFCoreFavoritosAppService_UnitTest : FavoritosAppService_UnitTest<TurisTrackEntityFrameworkCoreTestModule>
{
}
