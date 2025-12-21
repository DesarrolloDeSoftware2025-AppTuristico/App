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
            // ARRANGE
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
                // ACT
                var resultado = await _experienciaAppService.CrearExperienciaAsync(destinoId, comentario, fechaVisita);

                // ASSERT
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
            // ARRANGE
            // Generamos un ID random que sabemos que NO está en la base de datos
            var destinoIdInexistente = Guid.NewGuid();

            // ACT
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _experienciaAppService.CrearExperienciaAsync(
                    destinoIdInexistente,
                    "Comentario intento fallido",
                    DateTime.Now
                );
            });

            // ASSERT
            exception.Message.ShouldBe("El destino turístico no existe.");
        }

        // PRUEBA 1 EditarExperienciaAsync Prueba de Éxito, El dueño edita su propia experiencia
        [Fact]
        public async Task EditarExperienciaAsync_Deberia_Modificar_Correctamente()
        {
            // ARRANGE
            var destinoId = Guid.NewGuid();

            var experiencia = new ExperienciaDeViaje(destinoId, "Comentario Original", DateTime.Now.AddDays(-1));
            await _experienciaRepository.InsertAsync(experiencia);

            // ACT
            var nuevoComentario = "Comentario Editado";
            var nuevaFecha = DateTime.Now;

            var experienciaGuardada = await _experienciaRepository.FindAsync(d => d.DestinoId == destinoId);
            var experienciaId = experienciaGuardada.Id;

            var resultado = await _experienciaAppService.EditarExperienciaAsync(experienciaId, nuevoComentario, nuevaFecha);

            // ASSERT
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
            // ARRANGE
            var usuarioHackerId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            var experiencia = new ExperienciaDeViaje(destinoId, "Original", DateTime.Now);

            
            // 1. Insertamos siendo el DUEÑO (Admin)
            await _experienciaRepository.InsertAsync(experiencia);
                
            var experienciaGuardada = await _experienciaRepository.FindAsync(d => d.DestinoId == destinoId);
            var experienciaId = experienciaGuardada.Id;

            // 2. Intentamos editar con otro usuario
            using (SimularLogin(usuarioHackerId))
            {
                // ACT
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _experienciaAppService.EditarExperienciaAsync(experienciaId, "Hacked", DateTime.Now);
                });

                // ASSERT
                exception.Message.ShouldContain("No tienes permiso para editar");
            }
        }

        // PRUEBA 3 EditarExperienciaAsync: Prueba de Fallo, La experiencia no existe
        [Fact]
        public async Task EditarExperienciaAsync_SiNoExiste_Deberia_Lanzar_Excepcion()
        {
            // ARRANGE
            var idInexistente = Guid.NewGuid();

            // ACT
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _experienciaAppService.EditarExperienciaAsync(idInexistente, "Texto", DateTime.Now);
            });

            // ASSERT
            exception.Message.ShouldContain("no existe");
        }

        // PRUEBA 1 EliminarExperienciaAsync: Prueba de Éxito, El dueño elimina su propia experiencia
        [Fact]
        public async Task EliminarExperienciaAsync_ComoDuenio_Deberia_Eliminar_Correctamente()
        {
            // ARRANGE
            var destinoId = Guid.NewGuid();

            var experiencia = new ExperienciaDeViaje(destinoId, "Para borrar", DateTime.Now);

            await _experienciaRepository.InsertAsync(experiencia);

            var experienciaGuardada = await _experienciaRepository.FindAsync(d => d.DestinoId == destinoId);
            var experienciaId = experienciaGuardada.Id;

            // ACT
            var resultado = await _experienciaAppService.EliminarExperienciaAsync(experienciaId);

            // ASSERT 1: Mensaje de éxito
            resultado.ShouldContain("eliminada correctamente");

            // ASSERT 2: Verificamos en BD
            var experienciaBorrada = await _experienciaRepository.FindAsync(experienciaId);
            experienciaBorrada.ShouldBeNull();
        }

        // PRUEBA 2 EliminarExperienciaAsync: Prueba de Fallo, Intento de eliminar experiencia ajena
        [Fact]
        public async Task EliminarExperienciaAsync_SinPermisos_Deberia_Lanzar_Excepcion()
        {
            // ARRANGE
            var hackerId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            var experiencia = new ExperienciaDeViaje(destinoId, "Datos privados", DateTime.Now);

            // 1. El dueño (Admin) crea la experiencia
            await _experienciaRepository.InsertAsync(experiencia);

            var experienciaGuardada = await _experienciaRepository.FindAsync(d => d.DestinoId == destinoId);
            var experienciaId = experienciaGuardada.Id;

            using (SimularLogin(hackerId)) // Ahora nos logueamos con otro usuario
            {
                // ACT
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _experienciaAppService.EliminarExperienciaAsync(experienciaId);
                });

                // ASSERT
                exception.Message.ShouldContain("No tienes permiso");
            }
        }

        // PRUEBA 3 EliminarExperienciaAsync: Prueba de Fallo, La experiencia no existe
        [Fact]
        public async Task EliminarExperienciaAsync_SiNoExiste_Deberia_Lanzar_Excepcion()
        {
            // ARRANGE
            var idInexistente = Guid.NewGuid();

            // ACT
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _experienciaAppService.EliminarExperienciaAsync(idInexistente);
            });

            // ASSERT
            exception.Message.ShouldContain("no existe");
        }

        // PRUEBA 1 ObtenerExperienciasPorUsuarioAsync: Prueba de Éxito, Obtener experiencias propias
        [Fact]
        public async Task ObtenerExperienciasDeOtrosAsync_Deberia_Filtrar_Mis_Experiencias()
        {
            // ARRANGE
            var miUserId = Guid.NewGuid();
            var otroUserId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            // 1. Experiencia "Mía" (No debería aparecer en el resultado)
            var experienciaMia = new ExperienciaDeViaje(destinoId, "Mía", DateTime.Now);

            // 2. Experiencia de "Otro" (Sí debería aparecer)
            var experienciaDeOtro = new ExperienciaDeViaje(destinoId, "De otro", DateTime.Now);

            // Insertamos la mía simulando ser yo
            using (SimularLogin(miUserId))
            {
                await _experienciaRepository.InsertAsync(experienciaMia);
            }

            // Insertamos la del otro simulando ser el otro
            using (SimularLogin(otroUserId))
            {
                await _experienciaRepository.InsertAsync(experienciaDeOtro);
            }

            using (SimularLogin(miUserId)) // Me logueo yo para consultar
            {
                // ACT
                var resultado = await _experienciaAppService.ObtenerExperienciasDeOtrosAsync(destinoId);

                // ASSERT
                // 1. Verificamos que la lista no esté vacía
                resultado.ShouldNotBeNull();

                // 2. Debería haber solo 1 elemento (la del otro)
                resultado.Count.ShouldBe(1);

                // 3. Verificamos que el elemento sea efectivamente el del otro usuario
                resultado.ShouldContain(x => x.Comentario == "De otro");
            }
        }

        // PRUEBA 1 FiltrarPorSentimientoAsync: Filtrar experiencias por sentimiento Positivo
        [Fact]
        public async Task FiltrarPorSentimientoAsync_Deberia_Retornar_Solo_Positivas()
        {
            // ARRANGE
            var destinoId = Guid.NewGuid();

            // 1. Creamos una experiencia POSITIVA (usamos palabras clave: "hermoso", "genial")
            var expPositiva = new ExperienciaDeViaje(destinoId, "El paisaje es hermoso y el clima genial", DateTime.Now);

            // 2. Creamos una experiencia NEGATIVA (usamos palabras clave: "horrible", "sucio")
            var expNegativa = new ExperienciaDeViaje(destinoId, "Todo estaba horrible y muy sucio", DateTime.Now);

            // 3. Creamos una experiencia NEUTRAL (sin palabras clave)
            var expNeutral = new ExperienciaDeViaje(destinoId, "Llegamos al hotel a las 10 am", DateTime.Now);

            // Insertamos todas las experiencias
            await _experienciaRepository.InsertAsync(expPositiva);
            await _experienciaRepository.InsertAsync(expNegativa);
            await _experienciaRepository.InsertAsync(expNeutral);

            // ACT
            // Solicitamos solo las POSITIVAS
            var resultado = await _experienciaAppService.FiltrarPorSentimientoAsync(destinoId, SentimientoExperiencia.Positiva);

            // ASSERT
            resultado.ShouldNotBeNull();
            resultado.Count.ShouldBe(1); // Solo debe haber 1

            // Verificamos que sea la correcta
            resultado.First().Comentario.ShouldContain("hermoso");
        }

        // PRUEBA 2 FiltrarPorSentimientoAsync: Filtrar experiencias por sentimiento Negativo
        [Fact]
        public async Task FiltrarPorSentimientoAsync_Deberia_Retornar_Solo_Negativas()
        {
            // ARRANGE
            var destinoId = Guid.NewGuid();

            // Insertamos datos mixtos nuevamente
            var expPositiva = new ExperienciaDeViaje(destinoId, "Excelente servicio", DateTime.Now);
            var expNegativa1 = new ExperienciaDeViaje(destinoId, "Pesimo trato y comida mala", DateTime.Now);
            var expNegativa2 = new ExperienciaDeViaje(destinoId, "Hotel sucio y caro", DateTime.Now);

            await _experienciaRepository.InsertAsync(expPositiva);
            await _experienciaRepository.InsertAsync(expNegativa1);
            await _experienciaRepository.InsertAsync(expNegativa2);

            // ACT
            // Solicitamos solo las NEGATIVAS
            var resultado = await _experienciaAppService.FiltrarPorSentimientoAsync(destinoId, SentimientoExperiencia.Negativa);

            // ASSERT
            resultado.Count.ShouldBe(2); // Deberían venir las 2 negativas
            resultado.ShouldAllBe(x => x.Comentario.Contains("Pesimo") || x.Comentario.Contains("sucio"));
        }

        // PRUEBA 3 FiltrarPorSentimientoAsync: Caso en el que no hay coincidencias
        [Fact]
        public async Task FiltrarPorSentimientoAsync_SiNoHayCoincidencias_Deberia_Retornar_ListaVacia()
        {
            // ARRANGE
            var destinoId = Guid.NewGuid();

            // Solo insertamos una experiencia Neutral
            var expNeutral = new ExperienciaDeViaje(destinoId, "Fuimos en auto", DateTime.Now);

            await _experienciaRepository.InsertAsync(expNeutral);

            // ACT
            // Pedimos POSITIVAS (pero solo hay neutrales en la BD para ese destino)
            var resultado = await _experienciaAppService.FiltrarPorSentimientoAsync(destinoId, SentimientoExperiencia.Positiva);

            // ASSERT
            resultado.ShouldNotBeNull();
            resultado.ShouldBeEmpty(); // La lista debe estar vacía, no null
        }

        // PRUEBA 1 BuscarPorPalabraClaveAsync: Búsqueda exitosa con coincidencias
        [Fact]
        public async Task BuscarPorPalabraClaveAsync_Deberia_Traer_Solo_Coincidencias()
        {
            // ARRANGE
            var destinoId = Guid.NewGuid();
            var palabraClave = "montaña";

            // 1. Caso de coincidencia exacta
            var expExacta = new ExperienciaDeViaje(destinoId, "Me encantó la vista de la montaña", DateTime.Now);

            // 2. Caso de coincidencia parcial (ej. plural o dentro de una frase)
            var expParcial = new ExperienciaDeViaje(destinoId, "Hicimos esquí en las montañas nevadas", DateTime.Now);

            // 3. Caso de NO coincidencia (Control)
            var expSinCoincidencia = new ExperienciaDeViaje(destinoId, "Un día de playa soleado", DateTime.Now);

            // Insertamos los datos
            await _experienciaRepository.InsertAsync(expExacta);
            await _experienciaRepository.InsertAsync(expParcial);
            await _experienciaRepository.InsertAsync(expSinCoincidencia);

            // ACT
            // Realizamos la búsqueda
            var resultado = await _experienciaAppService.BuscarPorPalabraClaveAsync(palabraClave);

            // ASSERT
            // 1. Verificamos que traiga resultados
            resultado.ShouldNotBeNull();

            // 2. Debería traer 2 registros (la exacta y la parcial "montañas")
            resultado.Count.ShouldBe(2);

            // 3. Verificamos que los textos encontrados contengan la palabra
            resultado.ShouldContain(x => x.Comentario.Contains("vista de la montaña"));
            resultado.ShouldContain(x => x.Comentario.Contains("montañas nevadas"));

            // 4. Aseguramos que NO traiga el registro de "playa"
            resultado.ShouldNotContain(x => x.Comentario.Contains("playa"));
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
