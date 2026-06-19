namespace EcoPlantas.Services.LLM
{
    /// <summary>
    /// Router que selecciona el proveedor LLM según la configuración AI:Provider.
    /// - "Ollama" (por defecto) → OllamaService (local).
    /// - "OpenAI"               → OpenAiLlmService (cloud, para Render).
    /// Si AI:Provider no existe, usa "Ollama" para que el entorno local siga funcionando.
    /// </summary>
    public class LlmProviderService
    {
        private readonly IConfiguration _configuration;
        private readonly OllamaService _ollama;
        private readonly OpenAiLlmService _openai;

        public LlmProviderService(
            IConfiguration configuration,
            OllamaService ollama,
            OpenAiLlmService openai)
        {
            _configuration = configuration;
            _ollama = ollama;
            _openai = openai;
        }

        /// <summary>Nombre del proveedor configurado (por defecto "Ollama").</summary>
        public string ProviderConfigurado =>
            (_configuration["AI:Provider"] ?? "Ollama").Trim();

        /// <summary>Instancia del proveedor LLM activo según configuración.</summary>
        public ILlmService Actual =>
            ProviderConfigurado.Equals("OpenAI", StringComparison.OrdinalIgnoreCase)
                ? _openai
                : _ollama;

        public string Provider => Actual.Provider;
        public string Model => Actual.Model;
        public string? BaseUrl => Actual.BaseUrl;

        public Task<string> GenerarRespuestaAsync(string prompt) =>
            Actual.GenerarRespuestaAsync(prompt);

        public Task<(bool Disponible, string Mensaje)> ComprobarDisponibilidadAsync() =>
            Actual.ComprobarDisponibilidadAsync();
    }
}
