using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp.Application.Dtos;

namespace TurisTrack.APIExterna
{
    public interface IGeoDbDestinoService
    {
        Task<PagedResultDto<DestinoTuristicoDto>> BuscarDestinosAsync(BusquedaDestinoTuristicoDto input);
        Task<DestinoTuristicoDto> ObtenerDestinoPorIdAsync(int id);
        Task<List<DestinoTuristicoDto>> ListarDestinosPopularesAsync(int limit = 10);
    }
}
