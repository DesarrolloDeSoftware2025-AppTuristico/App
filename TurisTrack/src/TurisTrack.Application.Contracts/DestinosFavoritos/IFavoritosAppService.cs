using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos.Dtos;
using Volo.Abp.Application.Services;

namespace TurisTrack.DestinosTuristicos
{
    public interface IFavoritosAppService : IApplicationService
    {
        Task<String> AgregarFavoritoAsync(Guid destinoId);
        Task<String> EliminarFavoritoAsync(Guid destinoId);
        Task<List<DestinoFavoritoDto>> GetListaFavoritosAsync();
    }
}