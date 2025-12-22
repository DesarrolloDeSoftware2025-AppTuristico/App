using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TurisTrack.DestinosTuristicos
{
    public class FavoritosAppService : ApplicationService, IFavoritosAppService
    {
        private readonly IRepository<DestinoFavorito, Guid> _favoritosRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinosRepository;
        private readonly ICurrentUser _currentUser;

        public FavoritosAppService(
            IRepository<DestinoFavorito, Guid> favoritosRepository,
            IRepository<DestinoTuristico, Guid> destinosRepository,
            ICurrentUser currentUser)
        {
            _favoritosRepository = favoritosRepository;
            _destinosRepository = destinosRepository;
            _currentUser = currentUser;
        }

        // 6.1 Agregar destino a favoritos
        [Authorize]
        public async Task<String> AgregarFavoritoAsync(Guid destinoId)
        {
            var usuarioId = _currentUser.GetId();

            // Validar que el destino exista
            var destinoExiste = await _destinosRepository.AnyAsync(x => x.Id == destinoId);
            if (!destinoExiste)
            {
                throw new UserFriendlyException("El destino no existe.");
            }

            // Validar si ya es favorito para no duplicar
            var yaEsFavorito = await _favoritosRepository.AnyAsync(x => x.UsuarioId == usuarioId && x.DestinoId == destinoId);
            if (yaEsFavorito)
            {
                return "El destino turístico ya se encuentra en tus favoritos.";
            }

            var nuevoFavorito = new DestinoFavorito(destinoId, usuarioId);
            await _favoritosRepository.InsertAsync(nuevoFavorito);

            return "Destino turístico agregado a tus favoritos.";
        }

        // 6.2 Eliminar destino de favoritos
        [Authorize]
        public async Task<String> EliminarFavoritoAsync(Guid destinoId)
        {
            var usuarioId = _currentUser.GetId();

            var favorito = await _favoritosRepository.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.DestinoId == destinoId);

            if (favorito == null)
            { 
                return "El destino turístico no se encuentra en tus favoritos.";
            }

            await _favoritosRepository.DeleteAsync(favorito);

            return "Destino turístico eliminado de tus favoritos.";
        }

        // 6.3 Consultar lista personal de favoritos
        [Authorize]
        public async Task<List<DestinoFavoritoDto>> GetListaFavoritosAsync()
        {
            var usuarioId = _currentUser.GetId();

            // 1. Obtener los queryables
            var queryFavoritos = await _favoritosRepository.GetQueryableAsync();
            var queryDestinos = await _destinosRepository.GetQueryableAsync();

            // 2. Hacer un Join (INNER JOIN) entre Favoritos y Destinos filtrando por el usuario actual
            // Esto es más eficiente que traer los IDs y luego buscar los destinos
            var query = from fav in queryFavoritos
                        join dest in queryDestinos on fav.DestinoId equals dest.Id
                        where fav.UsuarioId == usuarioId && !dest.Eliminado
                        select dest;

            var listaDestinos = await AsyncExecuter.ToListAsync(query);

            // 3. Mapear a DTO (Usando AutoMapper si lo tienes configurado, o manual)
            return ObjectMapper.Map<List<DestinoTuristico>, List<DestinoFavoritoDto>>(listaDestinos);
        }
    }
}