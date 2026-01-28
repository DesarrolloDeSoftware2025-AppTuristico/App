using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace TurisTrack.Data;

/* This is used if database provider does't define
 * ITurisTrackDbSchemaMigrator implementation.
 */
public class NullTurisTrackDbSchemaMigrator : ITurisTrackDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
