using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TurisTrack.DestinosTuristicos
{
    public interface IDestinosAppService : IApplicationService
    {
        Task<List<DestinoTuristicoDto>> BuscarDestinosAsync(string nombre, string pais = null, string region = null);
        Task<DestinoTuristicoDto> ObtenerPorIdAsync(Guid id);
        Task<List<DestinoTuristicoDto>> ListarPopularesAsync();
        Task<DestinoTuristicoDto> GuardarDestinoAsync(CreateUpdateDestinoTuristicoDto input);
    }

}
