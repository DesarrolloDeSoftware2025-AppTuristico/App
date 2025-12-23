using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TurisTrack.Notificaciones
{
    public class NotificacionDto : EntityDto<Guid>
    {
        public Guid DestinoTuristicoId { get; set; }
        public string NombreDestino { get; set; } // Extra para el Front
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public TipoNotificacion Tipo { get; set; }
        public bool Leido { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
