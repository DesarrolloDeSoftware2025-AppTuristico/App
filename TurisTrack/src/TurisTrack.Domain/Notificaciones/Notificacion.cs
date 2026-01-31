using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TurisTrack.Notificaciones
{
    public class Notificacion : CreationAuditedEntity<Guid>
    {
        public Guid UserId { get; private set; } // El usuario que recibe la noti
        public Guid DestinoTuristicoId { get; private set; } // El destino relacionado
        public string Titulo { get; private set; }
        public string Mensaje { get; private set; }
        public TipoNotificacion Tipo { get; private set; }
        public bool Leido { get; private set; }

        protected Notificacion() { }

        public Notificacion(Guid userId, Guid destinoTuristicoId, string titulo, string mensaje, TipoNotificacion tipo)
        {
            UserId = userId;
            DestinoTuristicoId = destinoTuristicoId;
            Titulo = titulo; // Ej: "Alerta de Tormenta"
            Mensaje = mensaje; // Ej: "Se esperan lluvias fuertes en Cancún..."
            Tipo = tipo;
            Leido = false;
        }

        public void MarcarComoLeido()
        {
            Leido = true;
        }

        public void MarcarComoNoLeido()
        {
            Leido = false;
        }
    }
}