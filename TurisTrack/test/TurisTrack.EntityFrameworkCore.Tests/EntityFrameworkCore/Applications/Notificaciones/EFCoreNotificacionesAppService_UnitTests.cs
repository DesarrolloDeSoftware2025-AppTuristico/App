using TurisTrack.Notificaciones;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications.Notificaciones;

[Collection(TurisTrackTestConsts.CollectionDefinitionName)]

public class EFCoreNotificacionesAppService_UnitTests : NotificacionesAppService_UnitTests<TurisTrackEntityFrameworkCoreTestModule>
{
}
