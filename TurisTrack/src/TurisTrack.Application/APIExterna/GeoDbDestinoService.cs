using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using TurisTrack.DestinosTuristicos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;


namespace TurisTrack.APIExterna
{
    public class GeoDbDestinoService : ITransientDependency, IGeoDbDestinoService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";
        private const string AtributosAPI = "id,name,country,countryCode,region,regionCode,latitude,longitude,elevationMeters,population,timezone,type";
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        public GeoDbDestinoService(IHttpClientFactory httpClientFactory, IRepository<DestinoTuristico, Guid> destinoRepository)
        {
            _httpClient = httpClientFactory.CreateClient("GeoDbApi");
            _destinoRepository = destinoRepository;

        }


        // 3.1 Buscar destinos por nombre parcial o completo, opcional país o región y población mínima
        public async Task<PagedResultDto<DestinoTuristicoDto>> BuscarDestinosAsync(BusquedaDestinoTuristicoDto input)
        {
            // 1) Construir query para API externa
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["namePrefix"] = input.Nombre;
            query["limit"] = "100"; // Le pedimos más resultados y paginamos localmente
            query["fields"] = AtributosAPI;

            var url = $"{BaseUrl}/cities?{query}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Status Code: {response.StatusCode}");
                    Console.WriteLine($"Error: {errorContent}");
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<GeoDbCitySearchResult>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var destinos = new List<DestinoTuristicoDto>();

                // ---------------------------
                // 2) Convertir respuesta a DTO local
                // ---------------------------
                if (searchResult?.Data != null)
                {
                    foreach (var item in searchResult.Data)
                    {
                        destinos.Add(new DestinoTuristicoDto
                        {
                            IdAPI = item.Id,
                            Tipo = item.Type,
                            Nombre = item.Name,
                            Pais = item.Country,
                            CodigoPais = item.CountryCode,
                            Region = item.Region,
                            CodigoRegion = item.RegionCode,
                            MetrosDeElevacion = item.ElevationMeters,
                            Latitud = item.Latitude,
                            Longitud = item.Longitude,
                            Poblacion = item.Population,
                            ZonaHoraria = item.Timezone,
                            Eliminado = false
                        });
                    }
                }

                // ---------------------------
                // 3) Aplicar filtros locales
                // ---------------------------
                if (!string.IsNullOrWhiteSpace(input.Pais))
                {
                    destinos = destinos
                        .Where(d =>
                            (d.Pais != null && d.Pais.Contains(input.Pais, StringComparison.OrdinalIgnoreCase)) ||
                            (d.CodigoPais != null && d.CodigoPais.Contains(input.Pais, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(input.Region))
                {
                    destinos = destinos
                        .Where(d =>
                            (d.Region != null && d.Region.Contains(input.Region, StringComparison.OrdinalIgnoreCase)) ||
                            (d.CodigoRegion != null && d.CodigoRegion.Contains(input.Region, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                if (input.PoblacionMinima.HasValue && input.PoblacionMinima > 0)
                {
                    destinos = destinos
                        .Where(d => d.Poblacion >= input.PoblacionMinima.Value)
                        .ToList();
                }


                // ---------------------------
                // 5) Total antes de paginar
                // ---------------------------
                var totalCount = destinos.Count;

                // ---------------------------
                // 6) Aplicar paginación (local)
                // ---------------------------
                var pagedItems = destinos
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToList();

                // ---------------------------
                // 7) Devolver resultado compatible con Angular y ABP
                // ---------------------------
                return new PagedResultDto<DestinoTuristicoDto>(totalCount, pagedItems);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }



        /// 3.2 Obtener información detallada de un destino (por ID)
        public async Task<DestinoTuristicoDto> ObtenerDestinoPorIdAsync(int id)
        {
            // Solicitar solo los campos necesarios
            var url = $"{BaseUrl}/cities/{id}?fields={AtributosAPI}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                // Manejo especial para 404 (no encontrado)
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var detail = JsonSerializer.Deserialize<GeoDbCityDetailResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (detail?.Data == null) return null;

                return new DestinoTuristicoDto
                {
                    IdAPI = detail.Data.Id,
                    Tipo = detail.Data.Type,
                    Nombre = detail.Data.Name,
                    Pais = detail.Data.Country,
                    CodigoPais = detail.Data.CountryCode,
                    Region = detail.Data.Region,
                    CodigoRegion = detail.Data.RegionCode,
                    MetrosDeElevacion = detail.Data.ElevationMeters,
                    Latitud = detail.Data.Latitude,
                    Longitud = detail.Data.Longitude,
                    Poblacion = detail.Data.Population,
                    ZonaHoraria = detail.Data.Timezone,
                    Eliminado = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        /// 3.3 Listar destinos populares (Ciudades más pobladas)
        public async Task<List<DestinoTuristicoDto>> ListarDestinosPopularesAsync(int limit = 10)
        {
            // Solicitar solo los campos necesarios
            var url = $"{BaseUrl}/cities?sort=-population&limit={limit}&fields={AtributosAPI}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<GeoDbCitySearchResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var destinos = new List<DestinoTuristicoDto>();
                if (searchResult?.Data != null)
                {
                    foreach (var item in searchResult.Data)
                    {
                        destinos.Add(new DestinoTuristicoDto
                        {
                            IdAPI = item.Id,
                            Tipo = item.Type,
                            Nombre = item.Name,
                            Pais = item.Country,
                            CodigoPais = item.CountryCode,
                            Region = item.Region,
                            CodigoRegion = item.RegionCode,
                            MetrosDeElevacion = item.ElevationMeters,
                            Latitud = item.Latitude,
                            Longitud = item.Longitude,
                            Poblacion = item.Population,
                            ZonaHoraria = item.Timezone,
                            Eliminado = false
                        });
                    }
                }

                return destinos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        // Modelos auxiliares
        private class GeoDbCityResponse
        {
            public GeoDbCity Data { get; set; }
        }

        private class GeoDbCity
        {
            public string City { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }
            public string Region { get; set; }
            public string RegionCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int Population { get; set; }
            public int ElevationMeters { get; set; }
            public string Timezone { get; set; }
        }


        // --- Clases internas para mapear JSON ---
        private class GeoDbCitySearchResult
        {
            public List<GeoDbCityItem> Data { get; set; }
        }

        private class GeoDbCityDetailResult
        {
            public GeoDbCityItem Data { get; set; }
        }

        private class GeoDbCityItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }
            public string Region { get; set; }
            public string RegionCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int ElevationMeters { get; set; }
            public int Population { get; set; }
            public string Timezone { get; set; }
            public string Type { get; set; }
        }
    }

}