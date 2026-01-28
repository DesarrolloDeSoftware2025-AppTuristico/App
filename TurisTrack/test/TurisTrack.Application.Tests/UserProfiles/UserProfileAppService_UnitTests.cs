using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace TurisTrack.UserProfiles
{
    public abstract class UserProfileAppService_UnitTests<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IUserProfileAppService _userProfileAppService;
        private readonly IdentityUserManager _userManager;
        protected UserProfileAppService_UnitTests()
        {
            _userProfileAppService = GetRequiredService<IUserProfileAppService>();
            _userManager = GetRequiredService<IdentityUserManager>();
        }

        // 1.5 Prueba de Eliminar Cuenta Propia (Soft Delete)
        // No la implementamos ya que el UserManager.DeleteAsync ya hace un Soft Delete propio de ABP.


        // 1.6 Prueba de Ver Perfil Público
        [Fact]
        public async Task GetPublicProfileAsync_Deberia_Retornar_Datos_De_Un_Usuario()
        {
            // Arrange: Crear usuario
            var userId = Guid.NewGuid();
            var user = new IdentityUser(userId, "viajero1", "viajero@turistrack.com");
            user.Name = "Juan";
            user.Surname = "Perez";

            await _userManager.CreateAsync(user);

            await WithUnitOfWorkAsync(async () =>
            {
                // Act: Llamamos al servicio
                var result = await _userProfileAppService.GetPublicProfileAsync(userId);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBe(userId);
                result.UserName.ShouldBe("viajero1");
                result.Name.ShouldBe("Juan");
                result.Surname.ShouldBe("Perez");
            });
        }
    }
}
