using System.Threading.Tasks;

namespace TurisTrack.Data;

public interface ITurisTrackDbSchemaMigrator
{
    Task MigrateAsync();
}
