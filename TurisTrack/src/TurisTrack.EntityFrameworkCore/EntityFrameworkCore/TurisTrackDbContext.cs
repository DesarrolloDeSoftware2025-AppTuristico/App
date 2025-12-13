using Microsoft.EntityFrameworkCore;
using TurisTrack.CalificacionesDestinos;
using TurisTrack.DestinosTuristicos;
using TurisTrack.ExperienciasDeViajes;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Users;

namespace TurisTrack.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class TurisTrackDbContext :
    AbpDbContext<TurisTrackDbContext>,
    IIdentityDbContext
{
    private readonly ICurrentUser? _currentUser;

    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<DestinoTuristico> DestinosTuristicos { get; set; }
    public DbSet<CalificacionDestino> CalificacionesDestino { get; set; }
    public DbSet<ExperienciaDeViaje> ExperienciasDeViaje { get; set; }


    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext 
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext .
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    #endregion

    public TurisTrackDbContext(DbContextOptions<TurisTrackDbContext> options,
            ICurrentUser? currentUser = null)
        : base(options)
    {
        _currentUser = currentUser;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(TurisTrackConsts.DbTablePrefix + "YourEntities", TurisTrackConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        builder.Entity<DestinoTuristico>(b =>
        {
            b.ToTable(TurisTrackConsts.DbTablePrefix + "DestinosTuristicos", TurisTrackConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.IdAPI).IsRequired();
            b.Property(x => x.Tipo).IsRequired().HasMaxLength(100);
            b.Property(x => x.Nombre).IsRequired().HasMaxLength(200);
            b.Property(x => x.Pais).IsRequired().HasMaxLength(200);
            b.Property(x => x.CodigoPais).IsRequired(false).HasMaxLength(10);
            b.Property(x => x.Region).IsRequired().HasMaxLength(100);
            b.Property(x => x.CodigoRegion).IsRequired(false).HasMaxLength(10);
            b.Property(x => x.MetrosDeElevacion);
            b.Property(x => x.Latitud).IsRequired();
            b.Property(x => x.Longitud).IsRequired();
            b.Property(x => x.Poblacion).IsRequired();
            b.Property(x => x.ZonaHoraria).IsRequired(false).HasMaxLength(100);
            b.Property(x => x.Foto).IsRequired(false).HasMaxLength(500);
            b.Property(x => x.Eliminado);
        });

        builder.Entity<CalificacionDestino>(b =>
        {
            b.ToTable(TurisTrackConsts.DbTablePrefix + "CalificacionesDestinos", TurisTrackConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Puntuacion).IsRequired();
            b.Property(x => x.Comentario).IsRequired(false).HasMaxLength(1000);
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.DestinoTuristicoId).IsRequired();
            b.HasOne(x => x.DestinoTuristico)
                .WithMany()
                .HasForeignKey(x => x.DestinoTuristicoId).IsRequired();
            
            // Filtro global para el usuario actual
            b.HasQueryFilter(x => x.UserId == _currentUser.Id);
        });

        builder.Entity<ExperienciaDeViaje>(b =>
        {
            b.ToTable(TurisTrackConsts.DbTablePrefix + "ExperienciasDeViajes", TurisTrackConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.DestinoId).IsRequired();
            b.Property(x => x.Comentario).IsRequired().HasMaxLength(5000);
            b.Property(x => x.FechaVisita).IsRequired();
            b.Property(x => x.Sentimiento).IsRequired().HasDefaultValue(SentimientoExperiencia.Neutral);
        });
    }
}
