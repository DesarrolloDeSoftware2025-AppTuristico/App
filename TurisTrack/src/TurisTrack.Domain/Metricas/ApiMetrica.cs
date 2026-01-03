using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Auditing;

namespace TurisTrack.Metricas
{
    // Esta entidad guardará cada llamada individual a la API externa
    public class ApiMetrica : Entity<Guid>, IHasCreationTime
    {
        public string Endpoint { get; set; } // Ej: "BuscarDestinos"
        public string Parametros { get; set; } // Ej: "nombre=Paris"
        public int DuracionMs { get; set; } // Tiempo en milisegundos
        public bool FueExitoso { get; set; }
        public string? CodigoError { get; set; }
        public DateTime CreationTime { get; set; } // IHasCreationTime lo maneja ABP

        protected ApiMetrica() { }

        public ApiMetrica(string endpoint, string parametros, int duracionMs, bool fueExitoso, string? codigoError = null)
        {
            Endpoint = endpoint;
            Parametros = parametros;
            DuracionMs = duracionMs;
            FueExitoso = fueExitoso;
            CodigoError = codigoError;
        }
    }
}