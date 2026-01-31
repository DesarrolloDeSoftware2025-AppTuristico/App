using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace TurisTrack.Notificaciones
{
    public abstract class NotificacionesAppService_UnitTests<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly INotificacionesAppService _notificacionesAppService;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;
        public readonly ICurrentUser _currentUser;
        // Inyectamos el repositorio de usuarios para crear destinatarios de prueba
        private readonly IIdentityUserRepository _identityUserRepository;

        public NotificacionesAppService_UnitTests()
        {
            _notificacionesAppService = GetRequiredService<INotificacionesAppService>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
            _notificacionRepository = GetRequiredService<IRepository<Notificacion, Guid>>();
            _identityUserRepository = GetRequiredService<IIdentityUserRepository>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

        // PRUEBA 1 ReportarEventoEnDestinoAsync: Caso de Éxito, Generar notificaciones para TODOS los usuarios
        [Fact]
        public async Task ReportarEventoEnDestinoAsync_DatosValidos_Deberia_Crear_Notificaciones_Para_Todos()
        {
            // ARRANGE
            // 1. Crear e insertar un Destino
            var destino = new DestinoTuristico(
                idAPI: 800,
                tipo: "Playa",
                nombre: "Cancún",
                pais: "México",
                region: "Caribe",
                codigoPais: "MX",
                codigoRegion: "QR",
                metrosDeElevacion: 5,
                latitud: 21.16,
                longitud: -86.85,
                poblacion: 800000,
                zonaHoraria: "EST",
                foto: null
            );
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            // 2. Crear un par de usuarios extra para verificar el bucle
            // (ABP ya tiene al admin, así que con estos 2, debería haber al menos 3 usuarios)
            var user1 = new IdentityUser(Guid.NewGuid(), "usuario1", "u1@turistrack.com");
            var user2 = new IdentityUser(Guid.NewGuid(), "usuario2", "u2@turistrack.com");

            await _identityUserRepository.InsertAsync(user1, autoSave: true);
            await _identityUserRepository.InsertAsync(user2, autoSave: true);

            // Obtenemos el conteo total real de usuarios en la BD
            var totalUsuarios = await _identityUserRepository.GetCountAsync();

            // 3. Preparar el Input DTO
            var nuevoEvento = new CrearEventoDestinoDto
            {
                DestinoId = destino.Id,
                Titulo = "Alerta de Huracán",
                Mensaje = "Se aproxima tormenta, tomen precauciones.",
                Tipo = TipoNotificacion.Clima // Asumiendo que es un Enum
            };

            // ACT
            var resultado = await _notificacionesAppService.ReportarEventoEnDestinoAsync(nuevoEvento);

            // ASSERT
            // 1. Verificar mensaje de éxito
            resultado.ShouldContain("exitosamente");

            // 2. Verificar que se crearon notificaciones para TODOS
            var notificacionesEnBd = await _notificacionRepository.GetListAsync();

            // La cantidad de notificaciones debe ser igual a la cantidad de usuarios
            notificacionesEnBd.Count.ShouldBe((int)totalUsuarios);

            // 3. Verificar que los usuarios creados tengan su notificación
            notificacionesEnBd.ShouldContain(n => n.UserId == user1.Id && n.DestinoTuristicoId == destino.Id);
            notificacionesEnBd.ShouldContain(n => n.UserId == user2.Id && n.Titulo == "Alerta de Huracán");
        }

        // PRUEBA 2 ReportarEventoEnDestinoAsync: Caso de Fallo, Destino no existe
        [Fact]
        public async Task ReportarEventoEnDestinoAsync_DestinoInexistente_Deberia_Lanzar_Excepcion()
        {
            // ARRANGE
            var destinoIdInexistente = Guid.NewGuid();

            var input = new CrearEventoDestinoDto
            {
                DestinoId = destinoIdInexistente, // ID que no está en BD
                Titulo = "Evento Fantasma",
                Mensaje = "Esto no debería guardarse",
                Tipo = TipoNotificacion.Evento
            };

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _notificacionesAppService.ReportarEventoEnDestinoAsync(input);
            });

            // Validar mensaje
            exception.Message.ShouldContain("no existe");

            // Asegurarnos que NO se guardó ninguna notificación "sucia"
            var count = await _notificacionRepository.GetCountAsync();
            count.ShouldBe(0);
        }

        // PRUEBA 1 MarcarComoLeidaAsync: Caso de Éxito, cambiar estado a Leído
        [Fact]
        public async Task MarcarComoLeidaAsync_Deberia_Cambiar_Estado_A_Leido()
        {
            // ARRANGE
            var userId = _currentUser.GetId();

            // 1. Crear Destino
            var destino = new DestinoTuristico(901, "Ciudad", "Bogotá", "Colombia", "Suramérica", null, null, 2600, 4.7, -74.0, 8000000, null, null);
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            // 2. Crear Notificación (Por defecto se crea como NO Leída)
            var notificacion = new Notificacion(
                userId,
                destino.Id,
                "Bienvenida",
                "Hola mundo",
                TipoNotificacion.General
            );

            await _notificacionRepository.InsertAsync(notificacion, autoSave: true);
            var notificacionId = notificacion.Id;

            // ACT
            await _notificacionesAppService.MarcarComoLeidaAsync(notificacionId);

            // ASSERT
            var notificacionActualizada = await _notificacionRepository.GetAsync(notificacionId);

            // Verificamos que la propiedad Leido sea true
            notificacionActualizada.Leido.ShouldBeTrue();
        }

        // PRUEBA 2 MarcarComoNoLeidaAsync: Caso de Éxito, cambiar estado a No Leído
        [Fact]
        public async Task MarcarComoNoLeidaAsync_Deberia_Cambiar_Estado_A_NoLeido()
        {
            // ARRANGE
            var userId = _currentUser.GetId();

            // 1. Crear Destino
            var destino = new DestinoTuristico(902, "Ciudad", "Medellín", "Colombia", "Suramérica", null, null, 1495, 6.2, -75.5, 2500000, null, null);
            await _destinoRepository.InsertAsync(destino, autoSave: true);

            // 2. Crear Notificación
            var notificacion = new Notificacion(
                userId,
                destino.Id,
                "Aviso",
                "Mensaje leido previamente",
                TipoNotificacion.General
            );

            // La marcamos como leída manualmemte antes de guardar, 
            // para que en la base de datos inicie en estado "Leido = true"
            notificacion.MarcarComoLeido();

            await _notificacionRepository.InsertAsync(notificacion, autoSave: true);
            var notificacionId = notificacion.Id;

            // ACT
            await _notificacionesAppService.MarcarComoNoLeidaAsync(notificacionId);

            // ASSERT
            var notificacionActualizada = await _notificacionRepository.GetAsync(notificacionId);

            // Verificamos que volvió a estado false
            notificacionActualizada.Leido.ShouldBeFalse();
        }

        // PRUEBA 1 ObtenerMisNotificacionesAsync: Caso de Éxito, traer solo mis notificaciones, ordenadas y con nombre de destino
        [Fact]
        public async Task ObtenerMisNotificacionesAsync_Deberia_Traer_Solo_Las_Mias_Ordenadas_Y_Con_Nombre_Destino()
        {
            // ARRANGE
            var myUserId = _currentUser.GetId();
            var otherUserId = Guid.NewGuid();

            // 1. Crear Destinos
            var destParis = new DestinoTuristico(1001, "Ciudad", "Paris", "Francia", "EU", null, null, 35, 48.8, 2.3, 2000000, null, null);
            var destRoma = new DestinoTuristico(1002, "Ciudad", "Roma", "Italia", "EU", null, null, 20, 41.9, 12.5, 2800000, null, null);

            await _destinoRepository.InsertAsync(destParis, autoSave: true);
            await _destinoRepository.InsertAsync(destRoma, autoSave: true);

            // 2. Crear Notificaciones

            // Notif 1: Mía, Vieja - Destino Paris
            var notifMiaVieja = new Notificacion(myUserId, destParis.Id, "Oferta Vuelo", "Vuelo barato a Paris", TipoNotificacion.General);
            await _notificacionRepository.InsertAsync(notifMiaVieja, autoSave: true);

            // Pequeña espera para garantizar diferencia de tiempo en milisegundos
            await Task.Delay(50);

            // Notif 2: De OTRO usuario (No debería salir)
            var notifAjena = new Notificacion(otherUserId, destRoma.Id, "Privado", "No ver", TipoNotificacion.General);
            await _notificacionRepository.InsertAsync(notifAjena, autoSave: true);

            await Task.Delay(50);

            // Notif 3: Mía, Nueva (Hoy) - Destino Roma
            var notifMiaNueva = new Notificacion(myUserId, destRoma.Id, "Alerta Clima", "Lluvia en Roma", TipoNotificacion.Clima);
            await _notificacionRepository.InsertAsync(notifMiaNueva, autoSave: true);

            // ACT
            var resultado = await _notificacionesAppService.ObtenerMisNotificacionesAsync();

            // ASSERT
            resultado.ShouldNotBeNull();

            // 1. Validar Filtro (Solo las 2 mías)
            resultado.Count.ShouldBe(2);
            resultado.ShouldNotContain(n => n.Titulo == "Privado");

            // 2. Validar Orden (La nueva primero)
            resultado[0].Titulo.ShouldBe("Alerta Clima"); // La última insertada
            resultado[1].Titulo.ShouldBe("Oferta Vuelo");

            // 3. Validar "Join" de Nombre de Destino
            resultado[0].NombreDestino.ShouldBe("Roma");
            resultado[1].NombreDestino.ShouldBe("Paris");

        }

        // PRUEBA 2 ObtenerMisNotificacionesAsync: Caso Especial, destino asociado a notificación no existe
        [Fact]
        public async Task ObtenerMisNotificacionesAsync_SiDestinoNoExiste_Deberia_Mostrar_Texto_Por_Defecto()
        {
            // ARRANGE
            var myUserId = _currentUser.GetId();
            var idDestinoBorrado = Guid.NewGuid(); // ID que no existe en la tabla de destinos

            // Crear notificación apuntando a un destino fantasma
            var notificacion = new Notificacion(
                myUserId,
                idDestinoBorrado,
                "Error",
                "Destino desaparecido",
                TipoNotificacion.Evento
            );

            await _notificacionRepository.InsertAsync(notificacion, autoSave: true);

            // ACT
            var resultado = await _notificacionesAppService.ObtenerMisNotificacionesAsync();

            // ASSERT
            resultado.ShouldNotBeNull();
            resultado.Count.ShouldBe(1);

            resultado[0].NombreDestino.ShouldBe("Destino no disponible");
        }
    }
}
