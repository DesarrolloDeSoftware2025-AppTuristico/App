using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;
using System.Security.Claims;        // Para Claim, ClaimsIdentity, ClaimsPrincipal
using Volo.Abp.Security.Claims;      // Para ICurrentPrincipalAccessor y AbpClaimTypes

namespace TurisTrack.ExperienciasDeViajes
{
    public abstract class ExperienciaAppService_UnitTest<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IExperienciaAppService _experienciaAppService;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly IRepository<ExperienciaDeViaje, Guid> _experienciaRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        public ExperienciaAppService_UnitTest()
        {
            _experienciaAppService = GetRequiredService<IExperienciaAppService>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
            _experienciaRepository = GetRequiredService<IRepository<ExperienciaDeViaje, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }


        // PRUEBA 1 CrearExperienciaAsync: Crear experiencia correctamente
        [Fact]
        public async Task CrearExperienciaAsync_Deberia_Crear_Correctamente()
        {
            // Arrange
            // 1. Creamos un destino turístico válido para insertarlo en la BD de pruebas
            var destino = new DestinoTuristico(200, "Ruinas", "Machu Picchu", "Perú", "América del Sur", null, null, 450,
                500, 600, 450000, null, null);

            await _destinoRepository.InsertAsync(destino);

            // Datos para la nueva experiencia
            var fechaVisita = DateTime.Now;
            var comentario = "Fue una experiencia inolvidable.";

            var destinoGuardado = await _destinoRepository.FindAsync(d => d.Nombre == "Machu Picchu");
            var destinoId = destinoGuardado.Id;

            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var resultado = await _experienciaAppService.CrearExperienciaAsync(destinoId, comentario, fechaVisita);

                // Assert
                // 1. Verificar el mensaje de retorno
                resultado.ShouldBe("Experiencia de Viaje creada correctamente");

                // 2. Verificar que se guardó en la base de datos
                var experienciaEnBd = await _experienciaRepository.FirstOrDefaultAsync(e => e.DestinoId == destinoId);

                experienciaEnBd.ShouldNotBeNull();
                experienciaEnBd.Comentario.ShouldBe(comentario);
                experienciaEnBd.FechaVisita.ShouldBe(fechaVisita);
            });
        }

        // PRUEBA 2 CrearExperienciaAsync: Fallar si el destino no existe
        [Fact]
        public async Task CrearExperienciaAsync_ConDestinoInexistente_Deberia_Lanzar_Excepcion()
        {
            // Arrange
            // Generamos un ID random que sabemos que NO está en la base de datos
            var destinoIdInexistente = Guid.NewGuid();

            // Act
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _experienciaAppService.CrearExperienciaAsync(
                    destinoIdInexistente,
                    "Comentario intento fallido",
                    DateTime.Now
                );
            });

            // Assert
            exception.Message.ShouldBe("El destino turístico no existe.");
        }

        // PRUEBA 1 EditarExperienciaAsync Prueba de Éxito, El dueño edita su propia experiencia
        [Fact]
        public async Task EditarExperienciaAsync_Deberia_Modificar_Correctamente()
        {
            // Arrange
            var destinoId = Guid.NewGuid();

            var experiencia = new ExperienciaDeViaje(destinoId, "Comentario Original", DateTime.Now.AddDays(-1));
            await _experienciaRepository.InsertAsync(experiencia);

            // Act
            var nuevoComentario = "Comentario Editado";
            var nuevaFecha = DateTime.Now;

            var experienciaGuardada = await _experienciaRepository.FindAsync(d => d.DestinoId == destinoId);
            var experienciaId = experienciaGuardada.Id;

            var resultado = await _experienciaAppService.EditarExperienciaAsync(experienciaId, nuevoComentario, nuevaFecha);

            // Assert
            resultado.ShouldBe("Experiencia de Viaje modificada correctamente");
            // Verificamos en BD que los cambios persistan
            var experienciaDb = await _experienciaRepository.GetAsync(experienciaId);
            experienciaDb.Comentario.ShouldBe(nuevoComentario);
            experienciaDb.FechaVisita.ShouldBe(nuevaFecha);
        }

        // PRUEBA 2 EditarExperienciaAsync: Prueba de Fallo, Un usuario intenta editar la experiencia de OTRO
        [Fact]
        public async Task EditarExperienciaAsync_SinPermisos_Deberia_Lanzar_Excepcion()
        {
            // Arrange
            var usuarioDuenioId = Guid.NewGuid();
            var usuarioHackerId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            var experiencia = new ExperienciaDeViaje(destinoId, "Original", DateTime.Now);

            
            // 1. Insertamos siendo el DUEÑO (para que el CreatorId sea usuarioDuenioId)
            using (SimularLogin(usuarioDuenioId))
            {
                await _experienciaRepository.InsertAsync(experiencia);
            }
                
            var experienciaGuardada = await _experienciaRepository.FindAsync(d => d.DestinoId == destinoId);
            var experienciaId = experienciaGuardada.Id;

            // 2. Intentamos editar siendo el HACKER
            using (SimularLogin(usuarioHackerId))
            {
                // Act
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _experienciaAppService.EditarExperienciaAsync(experienciaId, "Hacked", DateTime.Now);
                });

                // Assert
                exception.Message.ShouldContain("No tienes permiso para editar");
            }
        }

        // PRUEBA 3 EditarExperienciaAsync: Prueba de Fallo, La experiencia no existe
        [Fact]
        public async Task EditarExperienciaAsync_SiNoExiste_Deberia_Lanzar_Excepcion()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();
            var cualquierUsuarioId = Guid.NewGuid();

            using (SimularLogin(cualquierUsuarioId))
            {
                // Act
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _experienciaAppService.EditarExperienciaAsync(idInexistente, "Texto", DateTime.Now);
                });

                // Assert
                exception.Message.ShouldContain("no existe");
            }
        }

        

        // --- MÉTODO MÁGICO PARA CAMBIAR DE USUARIO ---
        private IDisposable SimularLogin(Guid userId)
        {
            var claims = new List<Claim>
        {
            new Claim(AbpClaimTypes.UserId, userId.ToString()),
            new Claim(AbpClaimTypes.UserName, "usuario_test"),
            new Claim(AbpClaimTypes.Email, "test@turistrack.com")
        };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            // Esto cambia el usuario para todo el contexto de ABP dentro del using
            return _currentPrincipalAccessor.Change(principal);
        }
    }
}
