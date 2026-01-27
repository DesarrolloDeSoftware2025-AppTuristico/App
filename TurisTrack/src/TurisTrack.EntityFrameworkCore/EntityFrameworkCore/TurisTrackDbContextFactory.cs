using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TurisTrack.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class TurisTrackDbContextFactory : IDesignTimeDbContextFactory<TurisTrackDbContext>
{
    public TurisTrackDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        TurisTrackEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<TurisTrackDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));
        
        return new TurisTrackDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TurisTrack.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
