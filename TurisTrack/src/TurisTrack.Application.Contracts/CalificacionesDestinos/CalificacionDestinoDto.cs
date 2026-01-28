using System;
using Volo.Abp.Application.Dtos;

namespace TurisTrack.CalificacionesDestinos
{
    public class CalificacionDestinoDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid DestinoTuristicoId { get; set; }
        public int Puntuacion { get; set; }
        public string Comentario { get; set; }
        public DateTime CreationTime { get; set; }


    }
}
