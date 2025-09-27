using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TurisTrack.DestinosTuristicos
{
    public interface IDestinoTuristicoAppService : ICrudAppService<DestinoTuristicoDto, Guid>
    {
        Task<List<DestinoTuristicoDto>> BuscarDestinosAsync(string nombre, string? pais = null, string? region = null, int? poblacionMinima = null);
        Task<DestinoTuristicoDto> ObtenerPorIdAsync(Guid id);
        Task<List<DestinoTuristicoDto>> ListarPopularesAsync();
        Task<SaveResultDto> GuardarDestinoAsync(DestinoTuristicoDto destinoExterno);
    }

}
