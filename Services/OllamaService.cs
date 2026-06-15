using System.Text;
using System.Text.Json;

namespace EcoPlantas.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerarRespuestaAsync(string prompt)
        {
            var request = new
            {
                model = "llama3.2",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "http://localhost:11434/api/generate",
                content);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            using JsonDocument doc =
                JsonDocument.Parse(responseJson);

            return doc.RootElement
                      .GetProperty("response")
                      .GetString() ?? "";
        }
    }
}
