using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisTrack.DestinosTuristicos
{
    public class BusquedaDestinoTuristicoDto: PagedAndSortedResultRequestDto
    {
            public string Nombre { get; set; } = string.Empty;
            public string? Pais { get; set; }
            public string? Region { get; set; }
            public int? PoblacionMinima { get; set; }
    }

}
