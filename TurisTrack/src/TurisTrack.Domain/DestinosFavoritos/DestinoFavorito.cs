using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TurisTrack.DestinosTuristicos
{
    // Usamos CreationAuditedEntity para guardar automáticamente quién lo creó (CreatorId) y cuándo (CreationTime)
    public class DestinoFavorito : CreationAuditedEntity<Guid>
    {
        public Guid DestinoId { get; set; }
        public Guid UsuarioId { get; set; }

        protected DestinoFavorito() { }

        public DestinoFavorito(Guid destinoId, Guid usuarioId)
        {
            DestinoId = destinoId;
            UsuarioId = usuarioId;
        }
    }
}