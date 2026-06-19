using System.Text.Json;
using EcoPlantas.Models.Dto;

namespace EcoPlantas.Services
{
    /// <summary>
    /// Servicio que consume la Google Geocoding API desde el backend.
    /// La API key se lee de la configuración (clave "Mapa:ApiKey"), normalmente
    /// provista por la variable de entorno Mapa__ApiKey. Nunca se expone la key
    /// en las respuestas; todos los fallos se devuelven de forma controlada para
    /// no romper la aplicación.
    /// </summary>
    public class MapaService
    {
        private const string GeocodeUrl = "https://maps.googleapis.com/maps/api/geocode/json";

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MapaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// <summary>Indica si existe una API key configurada (sin revelar su valor).</summary>
        public bool GoogleMapsConfigurado => !string.IsNullOrWhiteSpace(ObtenerApiKey());

        /// <summary>Nombre del proveedor de mapas en uso.</summary>
        public string Provider => _configuration["Mapa:Provider"] ?? "GoogleMaps";

        private string? ObtenerApiKey() => _configuration["Mapa:ApiKey"];

        /// <summary>
        /// Convierte una dirección en coordenadas usando Google Geocoding API.
        /// Devuelve siempre un DTO; nunca lanza excepción hacia el controller.
        /// </summary>
        public async Task<GeocodeResponseDto> GeocodificarAsync(string direccion)
        {
            var respuesta = new GeocodeResponseDto
            {
                DireccionOriginal = direccion ?? string.Empty,
                Provider = Provider,
                Exito = false
            };

            if (string.IsNullOrWhiteSpace(direccion))
            {
                respuesta.Mensaje = "Debes indicar una dirección a geocodificar.";
                return respuesta;
            }

            var apiKey = ObtenerApiKey();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                respuesta.Mensaje = "Google Maps no está configurado. Define la variable de entorno Mapa__ApiKey.";
                return respuesta;
            }

            try
            {
                var url = $"{GeocodeUrl}?address={Uri.EscapeDataString(direccion)}&key={Uri.EscapeDataString(apiKey)}";

                using var httpResponse = await _httpClient.GetAsync(url);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    respuesta.Mensaje = $"El servicio de mapas respondió con estado HTTP {(int)httpResponse.StatusCode}.";
                    return respuesta;
                }

                var json = await httpResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var status = root.TryGetProperty("status", out var statusEl)
                    ? statusEl.GetString()
                    : "UNKNOWN";

                if (status != "OK")
                {
                    var errorMsg = root.TryGetProperty("error_message", out var errEl)
                        ? errEl.GetString()
                        : null;

                    respuesta.Mensaje = status == "ZERO_RESULTS"
                        ? "No se encontraron coordenadas para esa dirección."
                        : $"Google Geocoding devolvió estado '{status}'." +
                          (string.IsNullOrWhiteSpace(errorMsg) ? "" : $" {errorMsg}");
                    return respuesta;
                }

                var location = root.GetProperty("results")[0]
                    .GetProperty("geometry")
                    .GetProperty("location");

                respuesta.Latitud = location.GetProperty("lat").GetDouble();
                respuesta.Longitud = location.GetProperty("lng").GetDouble();
                respuesta.DireccionFormateada = root.GetProperty("results")[0]
                    .GetProperty("formatted_address").GetString() ?? string.Empty;
                respuesta.Exito = true;
                respuesta.Mensaje = "Coordenadas obtenidas correctamente.";
                return respuesta;
            }
            catch (Exception ex)
            {
                // No propagamos la excepción: la app no debe romperse si el API externo falla.
                respuesta.Mensaje = $"No se pudo contactar el servicio de mapas: {ex.Message}";
                return respuesta;
            }
        }
    }
}
