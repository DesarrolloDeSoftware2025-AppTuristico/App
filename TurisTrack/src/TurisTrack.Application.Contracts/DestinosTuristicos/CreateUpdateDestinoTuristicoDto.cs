using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisTrack.DestinosTuristicos
{
    public class CreateUpdateDestinoTuristicoDto : EntityDto<Guid>
    {
        public string NumeroCalle { get; set; }
        public string Calle { get; set; }
        public string Localidad { get; set; }
        public string Estado { get; set; }
        public string CodigoPostal { get; set; }
        public string Pais { get; set; }
        public string DireccionFormateada { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string TipoUbicacion { get; set; }
        public string Foto { get; set; }
    }
}
