using System;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace TurisTrack.ExperienciasDeViajes
{
    // Al heredar de FullAuditedAggregateRoot, ya tenemos:
    // - CreatorId (Usuario que creó)
    // - CreationTime (Fecha creación)
    // - LastModificationTime, LastModifierId, IsDeleted, etc.
    public class ExperienciaDeViaje : FullAuditedAggregateRoot<Guid>
    {
        public Guid DestinoId { get; set; }
        public string Comentario { get; private set; } // Hacemos el set privado para forzar el uso de métodos
        public DateTime FechaVisita { get; set; }
        public SentimientoExperiencia Sentimiento { get; private set; }

        protected ExperienciaDeViaje() { }

        public ExperienciaDeViaje(Guid destinoId, string comentario,DateTime fechaVisita)
        {
            DestinoId = destinoId;
            FechaVisita = fechaVisita;
            SetComentario(comentario); // Usamos el método para calcular sentimiento al inicio
        }

        // Método para actualizar comentario y recalcular sentimiento
        public void SetComentario(string comentario)
        {
            Comentario = Check.NotNullOrWhiteSpace(comentario, nameof(comentario));
            Sentimiento = AnalizarSentimiento(comentario);
        }

        // Lógica de análisis de texto
        private SentimientoExperiencia AnalizarSentimiento(string texto)
        {
            var textoNormalizado = texto.ToLower();

            // Diccionario básico (puedes expandirlo tanto como quieras)
            var palabrasPositivas = new[] { "excelente", "bueno", "hermoso", "increible", "recomiendo", "genial", "fantastico", "limpio", "seguro", "amable", "gusto", "encanto" };
            var palabrasNegativas = new[] { "malo", "horrible", "feo", "sucio", "inseguro", "caro", "terrible", "odio", "jamás", "pesimo", "lento", "grosero", "decepción" };

            int conteoPositivo = palabrasPositivas.Count(p => textoNormalizado.Contains(p));
            int conteoNegativo = palabrasNegativas.Count(p => textoNormalizado.Contains(p));

            if (conteoPositivo > conteoNegativo) return SentimientoExperiencia.Positiva;
            if (conteoNegativo > conteoPositivo) return SentimientoExperiencia.Negativa;

            return SentimientoExperiencia.Neutral;
        }
    }

}
