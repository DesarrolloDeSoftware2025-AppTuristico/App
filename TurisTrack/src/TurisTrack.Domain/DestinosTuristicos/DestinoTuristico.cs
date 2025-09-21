using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace TurisTrack.DestinosTuristicos
{
    public class DestinoTuristico : AggregateRoot<Guid>
    {
        public int IdAPI { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string CodigoPais { get; set; } //Ejemplo: US, AR, ES
        public string Region { get; set; }
        public string CodigoRegion { get; set; } //Ejemplo: NY (New York)
        public double MetrosDeElevacion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double Poblacion { get; set; }
        public string ZonaHoraria { get; set; }
        public string Foto { get; set; }
        public Boolean Eliminado { get; set; }
    }

}
