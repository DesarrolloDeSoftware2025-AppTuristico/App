using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TurisTrack.APIExterna;
using TurisTrack.DestinosTuristicos;
using TurisTrack.Metricas;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TurisTrack.DestinosTuristicos
{
    public class DestinoTuristicoAppService : ApplicationService
    {
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly IGeoDbDestinoService _geoDbService;
        private readonly IRepository<ApiMetrica, Guid> _metricasRepository;

        public DestinoTuristicoAppService(
            IRepository<DestinoTuristico, Guid> destinoRepository,
            IGeoDbDestinoService geoDbService,
            IRepository<ApiMetrica, Guid> metricasRepository)
        {
            _destinoRepository = destinoRepository;
            _geoDbService = geoDbService;
            _metricasRepository = metricasRepository;
        }

        // Método privado auxiliar para registrar la métrica sin repetir código
        private async Task RegistrarMetricaAsync(string endpoint, string parametros, Stopwatch sw, bool exito, string error = null)
        {
            sw.Stop();
            var metrica = new ApiMetrica(
                endpoint,
                parametros,
                (int)sw.ElapsedMilliseconds,
                exito,
                error
            );
            await _metricasRepository.InsertAsync(metrica);
        }

        // 3.1 Buscar destinos (Modificado con monitoreo)
        public async Task<List<DestinoTuristicoDto>> BuscarDestinosAsync(string nombre, string? pais = null, string? region = null,
            int? poblacionMinima = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new BusinessException("El nombre del destino no puede ser nulo o vacío.");
            }

            var sw = Stopwatch.StartNew();
            try
            {
                var result = await _geoDbService.BuscarDestinosAsync(nombre, pais, region, poblacionMinima);

                // Registrar éxito
                await RegistrarMetricaAsync("BuscarDestinos", $"nom:{nombre},pais:{pais}", sw, true);

                return result;
            }
            catch (Exception ex)
            {
                // Registrar fallo
                await RegistrarMetricaAsync("BuscarDestinos", $"nom:{nombre}", sw, false, ex.Message);
                throw;
            }
        }

        //  3.3 Obtener detalle de un destino (API externa) - CON MONITOREO
        public async Task<DestinoTuristicoDto> ObtenerDestinoPorIdAsync(int id)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var result = await _geoDbService.ObtenerDestinoPorIdAsync(id);

                // Registrar éxito
                await RegistrarMetricaAsync("ObtenerDestinoPorId", $"id:{id}", sw, true);

                return result;
            }
            catch (Exception ex)
            {
                // Registrar fallo
                await RegistrarMetricaAsync("ObtenerDestinoPorId", $"id:{id}", sw, false, ex.Message);
                throw;
            }
        }

        //  3.4 Listar destinos populares (API externa) - CON MONITOREO
        public async Task<List<DestinoTuristicoDto>> ListarDestinosPopularesAsync(int limit = 10) // Eliminé "limit = 10" si ya estaba definido en la interfaz, pero aquí lo dejo por defecto
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var result = await _geoDbService.ListarDestinosPopularesAsync(limit);

                // Registrar éxito
                await RegistrarMetricaAsync("ListarDestinosPopulares", $"limit:{limit}", sw, true);

                return result;
            }
            catch (Exception ex)
            {
                // Registrar fallo
                await RegistrarMetricaAsync("ListarDestinosPopulares", $"limit:{limit}", sw, false, ex.Message);
                throw;
            }
        }

        // 3.5 Guardar un destino en la base interna (SQL Server)
        public async Task<SaveResultDto> GuardarDestinoAsync(DestinoTuristicoDto destinoExterno) //int idApi)
        {
            // Buscar el destino en la API externa
            //var destinoExterno = await _geoDbService.ObtenerDestinoPorIdAsync(idApi);

            // Validar duplicados (Mismo Nombre y País)
            var existente = await _destinoRepository.FirstOrDefaultAsync(
                d => d.Nombre == destinoExterno.Nombre && d.Pais == destinoExterno.Pais
            );

            if (existente != null)
            {
                throw new BusinessException("TurisTrack:DestinoDuplicado")
                    .WithData("Nombre", destinoExterno.Nombre)
                    .WithData("Pais", destinoExterno.Pais)
                    .WithData("Message", $"El destino '{destinoExterno.Nombre}' en {destinoExterno.Pais} ya existe en la base interna.");
            }

            if ((destinoExterno.IdAPI == 0) || (destinoExterno.Nombre == null) || (destinoExterno.Tipo == null) || (destinoExterno.Pais == null)
                || (destinoExterno.Region == null))
            {
                throw new BusinessException("TurisTrack:CamposInvalidos")
                .WithData("Message", "Se deben completar los campos obligatorios.");
            }

            // Mapear con ObjectMapper al Entity
            var destino = ObjectMapper.Map<DestinoTuristicoDto, DestinoTuristico>(destinoExterno);

            // Insertar en la base
            var inserted = await _destinoRepository.InsertAsync(destino, autoSave: true);

            return new SaveResultDto
            {
                Success = true,
                Message = "Destino guardado correctamente",
                IdInterno = inserted.Id,
                IdApi = destinoExterno.IdAPI //idApi
            };
        }


        // Listar destinos guardados en la base local
        public async Task<List<DestinoTuristicoDtoPersistido>> ListarDestinosGuardadosAsync()
        {
            var destinos = await _destinoRepository.GetListAsync();
            var result = new List<DestinoTuristicoDtoPersistido>();

            foreach (var d in destinos)
            {
                result.Add(new DestinoTuristicoDtoPersistido
                {
                    Id = d.Id,
                    IdAPI = d.IdAPI,
                    Tipo = d.Tipo,
                    Nombre = d.Nombre,
                    Pais = d.Pais,
                    CodigoPais = d.CodigoPais,
                    Region = d.Region,
                    CodigoRegion = d.CodigoRegion,
                    MetrosDeElevacion = d.MetrosDeElevacion,
                    Latitud = d.Latitud,
                    Longitud = d.Longitud,
                    Poblacion = d.Poblacion,
                    ZonaHoraria = d.ZonaHoraria,
                    Eliminado = d.Eliminado
                });
            }

            return result;
        }
    }


    // DTO para guardar y mostrar lo que está en BD
    public class DestinoTuristicoDtoPersistido : EntityDto<Guid>
    {
        public Guid Id { get; set; }
        public int IdAPI { get; set; } 
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string CodigoPais { get; set; } // Ejemplo: US, AR, ES
        public string Region { get; set; }
        public string CodigoRegion { get; set; } // Ejemplo: NY (New York)
        public double MetrosDeElevacion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double Poblacion { get; set; }
        public string ZonaHoraria { get; set; }
        public string Foto { get; set; }
        public bool Eliminado { get; set; }
    }

}

