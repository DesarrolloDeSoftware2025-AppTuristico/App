using System;
using Volo.Abp.Application.Dtos;

namespace TurisTrack.DestinosTuristicos
{
    public class CalificacionDestinoDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public Guid DestinoTuristicoId { get; set; }
        public int Puntuacion { get; set; }
        public string Comentario { get; set; }


    }
}
