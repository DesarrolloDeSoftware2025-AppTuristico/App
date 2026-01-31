using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;

namespace TurisTrack.APIExterna
{
    public interface IGeoDbDestinoService
    {
        Task<List<DestinoTuristicoDto>> BuscarDestinosAsync(string nombre, string? pais = null, string? region = null, int? poblacionMinima = null);
        Task<DestinoTuristicoDto> ObtenerDestinoPorIdAsync(int id);
        Task<List<DestinoTuristicoDto>> ListarDestinosPopularesAsync(int limit = 10);
    }
}
