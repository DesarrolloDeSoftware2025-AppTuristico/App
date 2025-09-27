using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TurisTrack.DestinosTuristicos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TurisTrack.DestinosTuristicos
{
    public class DestinoTuristicoAppService : ApplicationService
    {
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly GeoDbDestinoService _geoDbService;

        public DestinoTuristicoAppService(
            IRepository<DestinoTuristico, Guid> destinoRepository,
            GeoDbDestinoService geoDbService)
        {
            _destinoRepository = destinoRepository;
            _geoDbService = geoDbService;
        }

        /// 3.1 Buscar destinos (API externa)
        public async Task<List<DestinoTuristicoDto>> BuscarDestinosAsync(string nombre, string? pais = null, string? region = null,
            int? poblacionMinima = null)
        {
            return await _geoDbService.BuscarDestinosAsync(nombre, pais, region, poblacionMinima);
        }

        /// 3.2 Obtener detalle de un destino (API externa)
        public async Task<DestinoTuristicoDto> ObtenerDestinoPorIdAsync(int id)
        {
            return await _geoDbService.ObtenerDestinoPorIdAsync(id);
        }

        /// 3.3 Listar destinos populares (API externa)
        public async Task<List<DestinoTuristicoDto>> ListarDestinosPopularesAsync(int limit = 10)
        {
            return await _geoDbService.ListarDestinosPopularesAsync(limit);
        }

        /// 3.4 Guardar un destino en la base interna (SQL Server)
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


        /// Listar destinos guardados en la base local
        public async Task<List<DestinoTuristicoDtoPersistido>> ListarDestinosGuardadosAsync()
        {
            var destinos = await _destinoRepository.GetListAsync();
            var result = new List<DestinoTuristicoDtoPersistido>();

            foreach (var d in destinos)
            {
                result.Add(new DestinoTuristicoDtoPersistido
                {
                    IdAPI = d.Id,
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
        public Guid IdAPI { get; set; }
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

