
export interface ApiMetricaResumenDto {
  totalPeticiones: number;
  peticionesExitosas: number;
  peticionesFallidas: number;
  tiempoPromedioMs: number;
  endpointMasLento?: string;
  tasaErroresPorcentaje: number;
}
