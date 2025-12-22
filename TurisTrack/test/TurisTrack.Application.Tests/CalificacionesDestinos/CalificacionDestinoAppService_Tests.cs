using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace TurisTrack.CalificacionesDestinos
{
    public abstract class CalificacionDestinoAppService_Tests<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ICalificacionDestinoAppService _calificacionAppService;
        private readonly DestinoTuristicoAppService _serviceDestinos;
        private readonly IRepository<CalificacionDestino, Guid> _calificacionRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IDataFilter _dataFilter;


        protected CalificacionDestinoAppService_Tests()
        {
            //Prueba de la funcionalidad de guardar un destino turistico
            _calificacionAppService = GetRequiredService<ICalificacionDestinoAppService>();
            _serviceDestinos = GetRequiredService<DestinoTuristicoAppService>();
            _calificacionRepository = GetRequiredService<IRepository<CalificacionDestino, Guid>>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
            _dataFilter = GetRequiredService<IDataFilter>();
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Destino_Guardado_Correctamente()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var existente = new DestinoTuristico(
                    102,
                    "Ciudad",
                    "Akhtala",
                    "Armenia",
                    "América del Sur",
                    "",
                    "",
                    25,
                    -34.6037,
                    -58.3816,
                    2890151
                );

                await _destinoRepository.InsertAsync(existente, autoSave: true);

                var destinos = await _serviceDestinos.ListarDestinosGuardadosAsync();

                var destinoId = destinos.First().Id;

                // Act
                var response = await _calificacionAppService.CrearCalificacionAsync(destinoId, 5, "Excelente destino turístico!");

                // Assert
                response.ShouldContain("creada exitosamente");
            });
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Destino_No_Encontrado()
        {
            await WithUnitOfWorkAsync(async () =>
            {

                // Act
                var exception = await Assert.ThrowsAsync<ApplicationException>(
                    async () => await _calificacionAppService.CrearCalificacionAsync(Guid.NewGuid(), 5, "Excelente destino turístico!"));

                // Assert
                exception.Message.ShouldContain("no encontrado");
            });
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Control_Rango_Puntuacion()
        {
            await WithUnitOfWorkAsync(async () =>
            {

                // Act
                var exception = await Assert.ThrowsAsync<ApplicationException>(
                    async () => await _calificacionAppService.CrearCalificacionAsync(Guid.NewGuid(), 7, "Excelente destino turístico!"));

                // Assert
                exception.Message.ShouldContain("entre 1 y 5");
            });
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Comentario_Opcional()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var existente = new DestinoTuristico(
                    102,
                    "Ciudad",
                    "Akhtala",
                    "Armenia",
                    "América del Sur",
                    "",
                    "",
                    25,
                    -34.6037,
                    -58.3816,
                    2890151
                );

                await _destinoRepository.InsertAsync(existente, autoSave: true);

                var destinos = await _serviceDestinos.ListarDestinosGuardadosAsync();

                var destinoId = destinos.First().Id;

                // Act
                var response = await _calificacionAppService.CrearCalificacionAsync(destinoId, 5);

                // Assert
                response.ShouldContain("creada exitosamente");
            });
        }

        [Fact]
        public async Task GuardarCalificacionAsync_Comprobar_Duplicado()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var existente = new DestinoTuristico(
                    102,
                    "Ciudad",
                    "Akhtala",
                    "Armenia",
                    "América del Sur",
                    "",
                    "",
                    25,
                    -34.6037,
                    -58.3816,
                    2890151
                );

                await _destinoRepository.InsertAsync(existente, autoSave: true);

                var destinos = await _serviceDestinos.ListarDestinosGuardadosAsync();

                var destinoId = destinos.First().Id;

                var response = await _calificacionAppService.CrearCalificacionAsync(destinoId, 5, "Excelente destino turístico!");

                // Act
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _calificacionAppService.CrearCalificacionAsync(destinoId, 5, "Excelente destino turístico!"));

                // Assert
                exception.Message.ShouldContain("calificado este destino anteriormente");
            });
        }

        // PRUEBA 1 EditarCalificacionAsync: Caso de Éxito, Modificación completa
        [Fact]
        public async Task EditarCalificacionAsync_DatosValidos_Deberia_Modificar_Todo()
        {
            // Arrange
            var userId = _currentUser.GetId();

            // 1. Crear Destino
            var destino = new DestinoTuristico(200, "Ruinas", "Machu Picchu", "Perú", "América del Sur", null, null, 450,
                500, 600, 450000, null, null);

            // IMPORTANTE: autoSave: true para que se genere el ID y se guarde en BD ya mismo
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            // No hace falta buscarlo en BD, el objeto 'destino' ya tiene su Id actualizado
            var destinoId = destino.Id;

            // 2. Crear Calificación
            var nuevaCalificacion = new CalificacionDestino(userId, destinoId, 3, "Regular");

            await _calificacionRepository.InsertAsync(nuevaCalificacion, autoSave: true);

            var calificacionId = nuevaCalificacion.Id;

            // Act
            // Cambiamos a 5 estrellas y "Excelente"
            var resultado = await _calificacionAppService.EditarCalificacionAsync(calificacionId, 5, "Excelente");
            resultado.ShouldContain("correctamente");

            // Assert
            var calificacionModificada = await _calificacionRepository.GetAsync(calificacionId);

            calificacionModificada.ShouldNotBeNull();
            calificacionModificada.Puntuacion.ShouldBe(5);
            calificacionModificada.Comentario.ShouldBe("Excelente");
        }

        // PRUEBA 2 EditarCalificacionAsync: Validación, Puntuación fuera de rango (1-5)
        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task EditarCalificacionAsync_PuntuacionFueraDeRango_Deberia_Lanzar_Excepcion(int puntuacionInvalida)
        {
            // Arrange
            var userId = _currentUser.GetId();

            // Crear e insertar Destino
            var destino = new DestinoTuristico(201, "Ruinas", "Tulum", "México", "Norteamérica", null, null, 10,
                20, 20, 1000, null, null);
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            var destinoId = destino.Id;

            // Crear calificación inicial válida
            var calificacion = new CalificacionDestino(userId, destinoId, 3, "Normal");
            await _calificacionRepository.InsertAsync(calificacion, autoSave: true);

            var calificacionId = calificacion.Id;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                // Intentamos actualizar con puntuación inválida
                await _calificacionAppService.EditarCalificacionAsync(calificacionId, puntuacionInvalida, "Cambio irrelevante");
            });

            exception.Message.ShouldContain("entre 1 y 5");
        }

        // PRUEBA 3 EditarCalificacionAsync: Caso Parcial: Solo Puntuación (Comentario null)
        [Fact]
        public async Task EditarCalificacionAsync_SoloPuntuacion_Deberia_Mantener_Comentario_Original()
        {
            // Arrange
            var userId = _currentUser.GetId();

            // Crear e insertar Destino
            var destino = new DestinoTuristico(202, "Playa", "Cancún", "México", "Norteamérica", null, null, 5,
                21, 86, 800000, null, null);
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            var destinoId = destino.Id;

            var calificacion = new CalificacionDestino(userId, destinoId, 2, "No cambiar esto");

            await _calificacionRepository.InsertAsync(calificacion, autoSave: true);
            var calificacionId = calificacion.Id;

            // Act
            // Pasamos nueva puntuación (4) pero comentario NULL
            await _calificacionAppService.EditarCalificacionAsync(calificacionId, 4, null);

            // Assert
            var calificacionDb = await _calificacionRepository.GetAsync(calificacionId);

            // Comprobamos si la puntuación cambió
            calificacionDb.Puntuacion.ShouldBe(4);
            // Comprobamos si el comentario sigue siendo el mismo
            calificacionDb.Comentario.ShouldBe("No cambiar esto");
        }

        // PRUEBA 3 EditarCalificacionAsync: Caso Parcial, Solo Comentario (Puntuación null)
        [Fact]
        public async Task EditarCalificacionAsync_SoloComentario_Deberia_Mantener_Puntuacion_Original()
        {
            // Arrange
            var userId = _currentUser.GetId();

            // Crear e insertar Destino
            var destino = new DestinoTuristico(203, "Montaña", "Everest", "Nepal", "Asia", null, null, 8848,
                27, 86, 0, null, null);
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            var calificacion = new CalificacionDestino(userId, destino.Id, 5, "Viejo comentario");

            await _calificacionRepository.InsertAsync(calificacion, autoSave: true);
            var calificacionId = calificacion.Id;

            // Act
            // Pasamos puntuación NULL pero nuevo comentario
            await _calificacionAppService.EditarCalificacionAsync(calificacionId, null, "Nuevo comentario");

            // Assert
            var calificacionDb = await _calificacionRepository.GetAsync(calificacionId);

            // Comprobamos si la puntuación sigue igual
            calificacionDb.Puntuacion.ShouldBe(5);
            // Comprobamos si el comentario cambió
            calificacionDb.Comentario.ShouldBe("Nuevo comentario");
        }

        // PRUEBA 1 EliminarCalificacionAsync: Caso de Éxito, Eliminar calificación propia
        [Fact]
        public async Task EliminarCalificacionAsync_ComoDuenio_Deberia_Eliminar_Correctamente()
        {
            // Arrange
            var userId = _currentUser.GetId();

            // 1. Crear Destino
            var destino = new DestinoTuristico(300, "Playa", "Varadero", "Cuba", "Caribe", null, null, 5,
                23, -81, 20000, null, null);
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            var destinoId = destino.Id;

            // 2. Crear Calificación asignada a mí (userId)
            var calificacion = new CalificacionDestino(userId, destinoId, 4, "A borrar");
            await _calificacionRepository.InsertAsync(calificacion, autoSave: true);

            var calificacionId = calificacion.Id;

            // Act
            var resultado = await _calificacionAppService.EliminarCalificacionAsync(calificacionId);

            // Assert
            resultado.ShouldContain("eliminada correctamente");

            // 2. Verificar que ya no existe en BD
            var calificacionBorrada = await _calificacionRepository.FindAsync(calificacionId);
            calificacionBorrada.ShouldBeNull();
        }

        // PRUEBA 2 EliminarCalificacionAsync: Caso de Fallo, Intentar eliminar calificación de OTRO
        [Fact]
        public async Task EliminarCalificacionAsync_SinPermisos_Deberia_Lanzar_Excepcion()
        {
            // Arrange
            var duenioId = Guid.NewGuid();          // Este usuario crea la calificación
            var hackerId = _currentUser.GetId();    // Este usuario intenta borrarla

            var destino = new DestinoTuristico(301, "Bosque", "Arrayanes", "Argentina", "Patagonia", null, null, 10,
                -40, -71, 5000, null, null);
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            var calificacionAjena = new CalificacionDestino(duenioId, destino.Id, 5, "Intocable");
            await _calificacionRepository.InsertAsync(calificacionAjena, autoSave: true);

            var calificacionId = calificacionAjena.Id;

            // Act & Assert
            // DESACTIVAMOS EL FILTRO IUserOwned, esto permite que el repositorio "vea" la entidad aunque no sea mía
            using (_dataFilter.Disable<IUserOwned>())
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _calificacionAppService.EliminarCalificacionAsync(calificacionId);
                });

                // Validamos el mensaje
                exception.Message.ShouldContain("No tienes permisos");
            }
        }

        // PRUEBA 1 ObtenerPromedioDestinoAsync: Caso de Éxito, Cálculo correcto de promedio y total
        [Fact]
        public async Task ObtenerPromedioDestinoAsync_Deberia_Calcular_Correctamente_Promedio_Y_Total()
        {
            // Arrange
            // 1. Crear e insertar un Destino
            var destino = new DestinoTuristico(400, "Histórico", "Roma", "Italia", "Europa", null, null, 20,
                41, 12, 3000000, null, null);

            await _destinoRepository.InsertAsync(destino, autoSave: true);

            // 2. Insertar varias calificaciones con distintos usuarios y puntuaciones
            var calif1 = new CalificacionDestino(Guid.NewGuid(), destino.Id, 5, "Excelente");
            var calif2 = new CalificacionDestino(Guid.NewGuid(), destino.Id, 4, "Muy bueno");
            var calif3 = new CalificacionDestino(Guid.NewGuid(), destino.Id, 2, "Regular");

            // Cálculo: (5 + 4 + 2) / 3 = 11 / 3 = 3.6666...
            // La lógica Math.Round(3.6666, 1) debería devolver 3.7

            await _calificacionRepository.InsertAsync(calif1, autoSave: true);
            await _calificacionRepository.InsertAsync(calif2, autoSave: true);
            await _calificacionRepository.InsertAsync(calif3, autoSave: true);

            // Act
            var resultado = await _calificacionAppService.ObtenerPromedioDestinoAsync(destino.Id);

            // Assert
            resultado.ShouldNotBeNull();

            // Verificamos el total
            resultado.TotalCalificaciones.ShouldBe(3);

            // Verificamos el promedio con redondeo
            resultado.Promedio.ShouldBe(3.7);
        }

        // PRUEBA 1 ObtenerComentariosPorDestinoAsync: Caso de Éxito, Retorno ordenado y con nombres de usuario
        [Fact]
        public async Task ObtenerComentariosPorDestinoAsync_Deberia_Retornar_Lista_Ordenada_Con_Nombres_De_Usuario()
        {
            // ARRANGE
            // Necesitamos el repositorio de usuarios de Identity para crear los usuarios "reales"
            var identityUserRepository = GetRequiredService<Volo.Abp.Identity.IIdentityUserRepository>();

            // 1. Crear Usuarios (User A y User B)
            var userA = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), "usuario_antiguo", "a@turistrack.com");
            var userB = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), "usuario_nuevo", "b@turistrack.com");

            await identityUserRepository.InsertAsync(userA, autoSave: true);
            await identityUserRepository.InsertAsync(userB, autoSave: true);

            // 2. Crear Destino
            var destino = new DestinoTuristico(500, "Museo", "Louvre", "Francia", "Europa", null, null, 35,
                2.3, 48.8, 1000000, null, null);
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            // 3. Crear Calificaciones
            // Calificación 1 (User A) - Insertada primero (será la más vieja)
            var califVieja = new CalificacionDestino(userA.Id, destino.Id, 5, "Primera visita");
            await _calificacionRepository.InsertAsync(califVieja, autoSave: true);

            // Pequeña pausa para asegurar que el reloj avance, por si la BD es muy rápida
            await Task.Delay(100);

            // Calificación 2 (User B) - Insertada después (será la más nueva)
            var califNueva = new CalificacionDestino(userB.Id, destino.Id, 3, "Segunda visita");
            await _calificacionRepository.InsertAsync(califNueva, autoSave: true);

            // ACT
            // Llamamos al método (que internamente desactiva el filtro IUserOwned para ver todo)
            var resultado = await _calificacionAppService.ObtenerComentariosPorDestinoAsync(destino.Id);

            // ASSERT
            resultado.ShouldNotBeNull();
            resultado.Count.ShouldBe(2);

            // 1. Verificamos el Orden (Descending por CreationTime -> La nueva primero)
            var primerResultado = resultado[0];
            var segundoResultado = resultado[1];

            primerResultado.Comentario.ShouldBe("Segunda visita");
            segundoResultado.Comentario.ShouldBe("Primera visita");

            // 2. Verificamos que se haya usado el UserName correctamente
            primerResultado.UserName.ShouldBe("usuario_nuevo");
            primerResultado.UserId.ShouldBe(userB.Id);

            segundoResultado.UserName.ShouldBe("usuario_antiguo");
            segundoResultado.UserId.ShouldBe(userA.Id);
        }

    }
}
