using Volo.Abp.Modularity;

namespace TurisTrack;

[DependsOn(
    typeof(TurisTrackDomainModule),
    typeof(TurisTrackTestBaseModule)
)]
public class TurisTrackDomainTestModule : AbpModule
{

}
