using System.Text;
using System.Text.Json;
using EcoPlantas.Services.LLM;

namespace EcoPlantas.Services
{
    public class OllamaService : ILlmService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OllamaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public string Provider => "Ollama";

        // URL base de Ollama. Configurable vía Ollama:BaseUrl (variable de entorno
        // Ollama__BaseUrl). Por defecto apunta a la instancia local de desarrollo.
        // Se recorta el '/' final para evitar dobles barras al construir la ruta.
        public string BaseUrl =>
            (_configuration["Ollama:BaseUrl"] ?? "http://localhost:11434").TrimEnd('/');

        // Modelo de Ollama. Configurable vía Ollama:Model (variable de entorno Ollama__Model).
        public string Model =>
            _configuration["Ollama:Model"] ?? "llama3.2";

        public async Task<string> GenerarRespuestaAsync(string prompt)
        {
            var url = $"{BaseUrl}/api/generate";

            var request = new
            {
                model = Model,
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(url, content);
            }
            catch (TaskCanceledException)
            {
                // Timeout: Ollama no respondió a tiempo.
                throw new OllamaUnavailableException(
                    "El servicio de IA (Ollama) no respondió a tiempo.");
            }
            catch (HttpRequestException ex)
            {
                // No se pudo establecer conexión (servicio caído, URL inalcanzable, etc.).
                throw new OllamaUnavailableException(
                    $"No se pudo conectar con Ollama en {BaseUrl}. {ex.Message}");
            }

            if (!response.IsSuccessStatusCode)
            {
                var detalle = (int)response.StatusCode == 404
                    ? $"El modelo '{Model}' no está disponible en Ollama o la URL no es correcta."
                    : $"Ollama respondió con estado HTTP {(int)response.StatusCode}.";
                throw new OllamaUnavailableException(detalle);
            }

            try
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);
                return doc.RootElement.GetProperty("response").GetString() ?? "";
            }
            catch (Exception ex)
            {
                throw new OllamaUnavailableException(
                    $"La respuesta de Ollama no tuvo el formato esperado. {ex.Message}");
            }
        }

        /// <summary>
        /// Diagnóstico ligero: comprueba si Ollama está accesible sin exponer secretos.
        /// Usa GET {BaseUrl}/api/tags con un timeout corto.
        /// </summary>
        public async Task<(bool Disponible, string Mensaje)> ComprobarDisponibilidadAsync()
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var response = await _httpClient.GetAsync($"{BaseUrl}/api/tags", cts.Token);

                return response.IsSuccessStatusCode
                    ? (true, "Ollama está disponible.")
                    : (false, $"Ollama respondió con estado HTTP {(int)response.StatusCode}.");
            }
            catch (TaskCanceledException)
            {
                return (false, "Ollama no respondió a tiempo (timeout).");
            }
            catch (Exception ex)
            {
                return (false, $"No se pudo conectar con Ollama en {BaseUrl}. {ex.Message}");
            }
        }
    }
}
