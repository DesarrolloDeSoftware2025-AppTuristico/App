using System;
using System.Collections.Generic;
using System.Security.Claims;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;

namespace TurisTrack;

public abstract class TurisTrackApplicationTestBase<TStartupModule> : TurisTrackTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    // --- MÉTODO MÁGICO PARA CAMBIAR DE USUARIO ---
    protected IDisposable SimularLogin(Guid userId)
    {
        var currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();

        var claims = new List<Claim>
        {
            new Claim(AbpClaimTypes.UserId, userId.ToString()),
            new Claim(AbpClaimTypes.UserName, "usuario_test"),
            new Claim(AbpClaimTypes.Email, "test@turistrack.com")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        // Esto cambia el usuario para todo el contexto de ABP dentro del using
        return currentPrincipalAccessor.Change(principal);
    }
}
