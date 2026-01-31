using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data; // Necesario para .GetProperty
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace TurisTrack.UserProfiles
{
    [Authorize]
    public class UserProfileAppService : TurisTrackAppService, IUserProfileAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        public UserProfileAppService(
            IdentityUserManager userManager,
            IRepository<IdentityUser, Guid> userRepository) 
        {
            _userManager = userManager;
            _userRepository = userRepository;
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

        // 1.7 Buscar usuarios por texto
        [AllowAnonymous]
        public async Task<List<PublicUserProfileDto>> SearchUsersAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new List<PublicUserProfileDto>();
            }

            var queryInput = input.ToLower();

            // 3. OBTENEMOS EL IQUERYABLE DEL REPOSITORIO (Forma segura en ABP)
            var queryable = await _userRepository.GetQueryableAsync();

            var query = queryable
                .Where(u => u.UserName.Contains(queryInput) ||
                            u.Name.Contains(queryInput) ||
                            u.Surname.Contains(queryInput) ||
                            (u.Name + " " + u.Surname).Contains(queryInput))
                .Take(20);

            // Ejecutamos usando AsyncExecuter
            var users = await AsyncExecuter.ToListAsync(query);

            // Mapeamos
            var result = new List<PublicUserProfileDto>();

            foreach (var user in users)
            {
                result.Add(new PublicUserProfileDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    Surname = user.Surname,
                    Foto = user.GetProperty<string>("Foto")
                });
            }

            return result;
        }
    }
}