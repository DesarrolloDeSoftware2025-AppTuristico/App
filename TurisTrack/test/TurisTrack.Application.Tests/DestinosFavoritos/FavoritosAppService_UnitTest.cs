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

namespace TurisTrack.DestinosFavoritos
{
    public abstract class FavoritosAppService_UnitTest<TStartupModule> : TurisTrackApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IFavoritosAppService _favoritosAppService;
        private readonly IRepository<DestinoTuristico, Guid> _destinosRepository;
        private readonly IRepository<DestinoFavorito, Guid> _favoritosRepository;
        private readonly ICurrentUser _currentUser;

        public FavoritosAppService_UnitTest()
        {
            _favoritosAppService = GetRequiredService<IFavoritosAppService>();
            _destinosRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
            _favoritosRepository = GetRequiredService<IRepository<DestinoFavorito, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

        // PRUEBA 1 AgregarFavoritoAsync: Caso de Éxito, El destino existe y se agrega correctamente
        [Fact]
        public async Task AgregarFavoritoAsync_DestinoExistente_Deberia_Agregar_Correctamente()
        {
            // Arrange
            var userId = _currentUser.GetId();

            // 1. Crear e insertar el destino turístico
            var destino = new DestinoTuristico(
                idAPI: 500,
                tipo: "Ciudad",
                nombre: "Tokio",
                pais: "Japón",
                region: "Asia",
                codigoPais: "JP",
                codigoRegion: "TK",
                metrosDeElevacion: 40,
                latitud: 35.6762,
                longitud: 139.6503,
                poblacion: 13960000,
                zonaHoraria: "JST",
                foto: null
            );

            // Importante: autoSave: true para que el repositorio encuentre el ID después
            await _destinosRepository.InsertAsync(destino, autoSave: true);
            var destinoId = destino.Id;

            // Act
            var mensaje = await _favoritosAppService.AgregarFavoritoAsync(destinoId);

            // Assert
            // 1. Verificar el mensaje de retorno
            mensaje.ShouldContain("agregado a tus favoritos");

            // 2. Verificar que se guardó en la base de datos
            var favoritoEnBd = await _favoritosRepository.FindAsync(x => x.DestinoId == destinoId && x.UsuarioId == userId);

            favoritoEnBd.ShouldNotBeNull();
            favoritoEnBd.DestinoId.ShouldBe(destinoId);
            favoritoEnBd.UsuarioId.ShouldBe(userId);
        }

        // PRUEBA 2 AgregarFavoritoAsync: Caso de Fallo, El destino no existe
        [Fact]
        public async Task AgregarFavoritoAsync_SiDestinoNoExiste_Deberia_Lanzar_Excepcion()
        {
            // Arrange
            // Generamos un ID aleatorio que no está en la BD
            var destinoIdInexistente = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _favoritosAppService.AgregarFavoritoAsync(destinoIdInexistente);
            });

            // Validamos el mensaje de la excepción
            exception.Message.ShouldContain("no existe");
        }

        // PRUEBA 1 EliminarFavoritoAsync: Caso de Éxito, El favorito existe y se elimina
        [Fact]
        public async Task EliminarFavoritoAsync_SiExiste_Deberia_Eliminarlo_Correctamente()
        {
            // Arrange
            var usuarioId = _currentUser.GetId();

            // 1. Crear e insertar un Destino
            var destino = new DestinoTuristico(
                idAPI: 600,
                tipo: "Playa",
                nombre: "Punta Cana",
                pais: "República Dominicana",
                region: "Caribe",
                codigoPais: "DO",
                codigoRegion: "PC",
                metrosDeElevacion: 0,
                latitud: 18.5601,
                longitud: 68.3725,
                poblacion: 43000,
                zonaHoraria: "AST",
                foto: null
            );

            await _destinosRepository.InsertAsync(destino, autoSave: true);
            var destinoId = destino.Id;

            // 2. Insertar el registro de Favorito (Simulamos que ya lo tenía agregado)
            var favorito = new DestinoFavorito(destinoId, usuarioId);
            await _favoritosRepository.InsertAsync(favorito, autoSave: true);

            // Act
            var resultado = await _favoritosAppService.EliminarFavoritoAsync(destinoId);

            // Assert
            // Verificar el mensaje de éxito
            resultado.ShouldContain("eliminado de tus favoritos");

            // Verificar que ya no exista en la base de datos
            var favoritoEnBd = await _favoritosRepository.FindAsync(favorito.Id);
            favoritoEnBd.ShouldBeNull();
        }

        // PRUEBA 2 EliminarFavoritoAsync: Caso Alternativo, El favorito no existe
        [Fact]
        public async Task EliminarFavoritoAsync_SiNoExiste_Deberia_Retornar_Mensaje_Informativo()
        {
            // Arrange
            // Usamos un ID cualquiera que no esté en favoritos
            var destinoIdInexistente = Guid.NewGuid();

            // Act
            var resultado = await _favoritosAppService.EliminarFavoritoAsync(destinoIdInexistente);

            // Assert
            // Validamos que retorne el mensaje de aviso
            resultado.ShouldContain("no se encuentra en tus favoritos");
        }

        // PRUEBA 1 GetListaFavoritosAsync: Caso de Éxito, Retorna la lista correcta de favoritos
        [Fact]
        public async Task GetListaFavoritosAsync_Deberia_Retornar_Solo_Mis_Destinos_Activos()
        {
            // ARRANGE
            var myUserId = _currentUser.GetId();
            var otherUserId = Guid.NewGuid();

            // 1. Crear Destinos
            // A. Destino Válido (Paris)
            var destParis = new DestinoTuristico(701, "Ciudad", "Paris", "Francia", "Europa", null, null, 35, 48.8, 2.3, 2000000, null, null);

            // B. Destino Eliminado (Londres) - Simulamos borrado lógico manual o propiedad Eliminado=true
            var destLondon = new DestinoTuristico(702, "Ciudad", "London", "UK", "Europa", null, null, 15, 51.5, -0.1, 8000000, null, null);
            destLondon.Eliminado = true;

            // C. Destino Activo (Madrid) - Para probar que no traiga favoritos ajenos
            var destMadrid = new DestinoTuristico(703, "Ciudad", "Madrid", "España", "Europa", null, null, 650, 40.4, -3.7, 3000000, null, null);

            // Guardamos los destinos
            await _destinosRepository.InsertAsync(destParis, autoSave: true);
            await _destinosRepository.InsertAsync(destLondon, autoSave: true);
            await _destinosRepository.InsertAsync(destMadrid, autoSave: true);

            // 2. Crear las relaciones de Favoritos
            // Caso 1: Mío y activo -> DEBERÍA SALIR
            await _favoritosRepository.InsertAsync(new DestinoFavorito(destParis.Id, myUserId), autoSave: true);

            // Caso 2: Mío pero eliminado -> NO DEBERÍA SALIR
            await _favoritosRepository.InsertAsync(new DestinoFavorito(destLondon.Id, myUserId), autoSave: true);

            // Caso 3: Activo pero de otro usuario -> NO DEBERÍA SALIR
            await _favoritosRepository.InsertAsync(new DestinoFavorito(destMadrid.Id, otherUserId), autoSave: true);

            // ACT
            var resultado = await _favoritosAppService.GetListaFavoritosAsync();

            // ASSERT
            resultado.ShouldNotBeNull();

            // Solo debería haber 1 elemento (Paris)
            resultado.Count.ShouldBe(1);

            // Verificamos que sea el correcto
            resultado[0].Nombre.ShouldBe("Paris");
        }
    }
}
