using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using OpenIddict.Abstractions;

namespace TurisTrack
{
    public class TurisTrackAuthServerDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly ILogger<TurisTrackAuthServerDataSeedContributor> _logger;
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IConfiguration _configuration;

        public TurisTrackAuthServerDataSeedContributor(
            ILogger<TurisTrackAuthServerDataSeedContributor> logger,
            IOpenIddictApplicationManager applicationManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _applicationManager = applicationManager;
            _configuration = configuration;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            const string clientId = "TurisTrack_Swagger";

            var existingClient = await _applicationManager.FindByClientIdAsync(clientId);
            var authority = _configuration["AuthServer:Authority"] ?? "https://localhost:44340";
            var redirectUri = new Uri($"{authority}/swagger/oauth2-redirect.html");

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                DisplayName = "Swagger UI",
                ClientType = OpenIddictConstants.ClientTypes.Public,
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                RedirectUris = { redirectUri },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "TurisTrack"
                }
            };

            if (existingClient == null)
            {
                await _applicationManager.CreateAsync(descriptor);
                _logger.LogInformation("✅ Cliente 'TurisTrack_Swagger' registrado correctamente.");
            }
            else
            {
                await _applicationManager.UpdateAsync(existingClient, descriptor);
                _logger.LogInformation("🔄 Cliente 'TurisTrack_Swagger' actualizado correctamente.");
            }
        }
    }
}

