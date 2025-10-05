using Volo.Abp.Modularity;

namespace TurisTrack;

[DependsOn(
    typeof(TurisTrackApplicationModule),
    typeof(TurisTrackDomainTestModule)
)]
public class TurisTrackApplicationTestModule : AbpModule
{

}
