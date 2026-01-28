using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TurisTrack.Data;
using Volo.Abp.DependencyInjection;

namespace TurisTrack.EntityFrameworkCore;

public class EntityFrameworkCoreTurisTrackDbSchemaMigrator
    : ITurisTrackDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreTurisTrackDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the TurisTrackDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<TurisTrackDbContext>()
            .Database
            .MigrateAsync();
    }
}
