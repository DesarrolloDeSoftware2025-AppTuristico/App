using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TurisTrack.UserProfiles
{
    public interface IUserProfileAppService : IApplicationService
    {
        // 1.5 Eliminar propia cuenta
        Task DeleteMyAccountAsync();

        // 1.6 Consultar perfil público
        Task<PublicUserProfileDto> GetPublicProfileAsync(Guid userId);
    }
}