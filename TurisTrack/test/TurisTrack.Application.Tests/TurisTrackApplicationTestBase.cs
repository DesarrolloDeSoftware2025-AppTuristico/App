using Volo.Abp.Modularity;

namespace TurisTrack;

public abstract class TurisTrackApplicationTestBase<TStartupModule> : TurisTrackTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
