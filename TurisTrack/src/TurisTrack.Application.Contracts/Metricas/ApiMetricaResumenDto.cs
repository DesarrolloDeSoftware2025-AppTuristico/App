namespace TurisTrack.Metricas
{
    public class ApiMetricaResumenDto
    {
        public int TotalPeticiones { get; set; }
        public int PeticionesExitosas { get; set; }
        public int PeticionesFallidas { get; set; }
        public double TiempoPromedioMs { get; set; }
        public string EndpointMasLento { get; set; }
        public double TasaErroresPorcentaje { get; set; }
    }
}