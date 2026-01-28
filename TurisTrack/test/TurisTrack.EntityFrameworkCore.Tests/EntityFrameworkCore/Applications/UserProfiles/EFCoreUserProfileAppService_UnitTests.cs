using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.Tests.DestinosTuristicos;
using TurisTrack.UserProfiles;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications.UserProfiles;

[Collection(TurisTrackTestConsts.CollectionDefinitionName)]

public class EFCoreUserProfileAppService_UnitTests : UserProfileAppService_UnitTests<TurisTrackEntityFrameworkCoreTestModule>
{
}

