using Volo.Abp.Modularity;

namespace TurisTrack;

/* Inherit from this class for your domain layer tests. */
public abstract class TurisTrackDomainTestBase<TStartupModule> : TurisTrackTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
