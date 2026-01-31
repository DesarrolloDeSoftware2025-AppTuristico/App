using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using System;


namespace TurisTrack;

[DependsOn(
    typeof(TurisTrackDomainModule),
    typeof(TurisTrackApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class TurisTrackApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<TurisTrackApplicationModule>();
        });

        context.Services.AddHttpClient("GeoDbApi", client =>
        {
            client.BaseAddress = new Uri("https://wft-geo-db.p.rapidapi.com/v1/geo/");
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "4d859b8aa9msh2c6d4cd6720db61p1751e1jsnb37f58aa3269");
        });

    }

}
