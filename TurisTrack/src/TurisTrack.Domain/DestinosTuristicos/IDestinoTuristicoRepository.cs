using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace TurisTrack.DestinosTuristicos
{
    public interface IDestinoTuristicoRepository : IRepository<DestinoTuristico, Guid>
    {
        Task<List<DestinoTuristico>> GetByCountryAsync(string country);
    }

}
