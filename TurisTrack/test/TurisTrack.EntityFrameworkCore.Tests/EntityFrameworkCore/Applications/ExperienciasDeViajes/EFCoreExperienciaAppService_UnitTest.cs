using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.ExperienciasDeViajes;
using TurisTrack.UserProfiles;
using Xunit;

namespace TurisTrack.EntityFrameworkCore.Applications.ExperienciasDeViajes;

[Collection(TurisTrackTestConsts.CollectionDefinitionName)]

public class EFCoreExperienciaAppService_UnitTest : ExperienciaAppService_UnitTest<TurisTrackEntityFrameworkCoreTestModule>
{
}
