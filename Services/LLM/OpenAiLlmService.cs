using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EcoPlantas.Services.LLM
{
    /// <summary>
    /// Proveedor LLM en la nube usando la Chat Completions API de OpenAI.
    /// Pensado para producción (Render), donde Ollama local no es accesible.
    /// La API key se lee de configuración (OpenAI:ApiKey) y nunca se expone.
    /// </summary>
    public class OpenAiLlmService : ILlmService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OpenAiLlmService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public string Provider => "OpenAI";

        public string Model => _configuration["OpenAI:Model"] ?? "gpt-4o-mini";

        // Se recorta el '/' final para evitar dobles barras al construir la ruta.
        public string? BaseUrl =>
            (_configuration["OpenAI:BaseUrl"] ?? "https://api.openai.com/v1").TrimEnd('/');

        private string? ApiKey => _configuration["OpenAI:ApiKey"];

        private bool Configurado => !string.IsNullOrWhiteSpace(ApiKey);

        public async Task<string> GenerarRespuestaAsync(string prompt)
        {
            if (!Configurado)
                throw new LlmUnavailableException(
                    "OpenAI no está configurado. Define la variable de entorno OpenAI__ApiKey.");

            var url = $"{BaseUrl}/chat/completions";

            var request = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(request);

            using var httpReq = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(httpReq);
            }
            catch (TaskCanceledException)
            {
                throw new LlmUnavailableException("OpenAI no respondió a tiempo (timeout).");
            }
            catch (HttpRequestException ex)
            {
                throw new LlmUnavailableException($"No se pudo conectar con OpenAI. {ex.Message}");
            }

            if (!response.IsSuccessStatusCode)
            {
                var detalle = (int)response.StatusCode switch
                {
                    401 => "La API key de OpenAI no es válida o está ausente.",
                    429 => "Se alcanzó el límite de uso o la cuota de OpenAI.",
                    404 => $"El modelo '{Model}' no existe o la URL de OpenAI no es correcta.",
                    _   => $"OpenAI respondió con estado HTTP {(int)response.StatusCode}."
                };
                throw new LlmUnavailableException(detalle);
            }

            try
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);
                return doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";
            }
            catch (Exception ex)
            {
                throw new LlmUnavailableException(
                    $"La respuesta de OpenAI no tuvo el formato esperado. {ex.Message}");
            }
        }

        public async Task<(bool Disponible, string Mensaje)> ComprobarDisponibilidadAsync()
        {
            if (!Configurado)
                return (false, "OpenAI no está configurado. Define la variable de entorno OpenAI__ApiKey.");

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var req = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/models");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                var response = await _httpClient.SendAsync(req, cts.Token);

                return response.IsSuccessStatusCode
                    ? (true, "OpenAI está disponible.")
                    : (false, $"OpenAI respondió con estado HTTP {(int)response.StatusCode}.");
            }
            catch (TaskCanceledException)
            {
                return (false, "OpenAI no respondió a tiempo (timeout).");
            }
            catch (Exception ex)
            {
                return (false, $"No se pudo conectar con OpenAI. {ex.Message}");
            }
        }
    }
}
