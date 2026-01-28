using System;
using Volo.Abp.Application.Dtos;

namespace TurisTrack.DestinosTuristicos.Dtos
{
    public class DestinoFavoritoDto : EntityDto<Guid>
    {
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}