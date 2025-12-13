using System;
using TurisTrack.DestinosTuristicos;
using Volo.Abp.Domain.Entities;

namespace TurisTrack.CalificacionesDestinos
{
    public class CalificacionDestino : Entity<Guid>, IUserOwned
    {
        public Guid UserId { get; set; } // usuario autenticado
        public Guid DestinoTuristicoId { get; set; }
        public int Puntuacion { get; set; } // 1–5 estrellas
        public string Comentario { get; set; }

        public DestinoTuristico DestinoTuristico { get; set; }

        private CalificacionDestino() { } // EF Core

        public CalificacionDestino(Guid userId, Guid destinoId, int puntuacion, string comentario)
        {
            UserId = userId;
            DestinoTuristicoId = destinoId;
            Puntuacion = puntuacion;
            Comentario = comentario;
        }
    }
}
