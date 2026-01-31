using TurisTrack.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace TurisTrack.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(TurisTrackEntityFrameworkCoreModule),
    typeof(TurisTrackApplicationContractsModule)
)]
public class TurisTrackDbMigratorModule : AbpModule
{
}
