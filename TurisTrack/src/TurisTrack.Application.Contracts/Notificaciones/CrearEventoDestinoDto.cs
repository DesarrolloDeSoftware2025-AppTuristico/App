using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisTrack.Notificaciones
{
    public class CrearEventoDestinoDto
    {
        public Guid DestinoId { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public TipoNotificacion Tipo { get; set; }
        
    }
}
