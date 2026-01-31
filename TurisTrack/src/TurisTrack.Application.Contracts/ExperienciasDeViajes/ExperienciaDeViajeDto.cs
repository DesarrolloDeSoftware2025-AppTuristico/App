using System;
using TurisTrack.ExperienciasDeViajes;
using Volo.Abp.Application.Dtos;

namespace TurisTrack.Experiencias.Dtos
{
    public class ExperienciaDeViajeDto : EntityDto<Guid>
    {
        public Guid DestinoId { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaVisita { get; set; }
        public SentimientoExperiencia Sentimiento { get; set; }

        // Campos de auditoría útiles para mostrar "Cuándo se escribió" y "Quién"
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
    }
}