using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data; // Necesario para .GetProperty
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace TurisTrack.UserProfiles
{
    [Authorize]
    public class UserProfileAppService : TurisTrackAppService, IUserProfileAppService
    {
        private readonly IdentityUserManager _userManager;

        public UserProfileAppService(IdentityUserManager userManager)
        {
            _userManager = userManager;
        }

        // 1.5 Eliminar cuenta propia
        public async Task DeleteMyAccountAsync()
        {
            if (CurrentUser.Id == null)
            {
                throw new UserFriendlyException("No estás logueado.");
            }

            var userId = CurrentUser.Id.Value;
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new UserFriendlyException("Usuario no encontrado.");
            }

            // 2. Eliminamos el usuario
            // Usamos un "Soft Delete" (IsDeleted = true).
            // El usuario ya no podrá iniciar sesión, pero los datos persisten en BD por seguridad.
            (await _userManager.DeleteAsync(user)).CheckErrors();
        }

        // 1.6 Ver perfil público de otros
        // Permitimos acceso anónimo para que cualquiera pueda ver perfiles 
        [AllowAnonymous]
        public async Task<PublicUserProfileDto> GetPublicProfileAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new UserFriendlyException("El usuario que buscas no existe.");
            }

            // Mapeo manual para asegurar que tomamos la propiedad extra "Foto"
            return new PublicUserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                Foto = user.GetProperty<string>("Foto")
            };
        }
    }
}