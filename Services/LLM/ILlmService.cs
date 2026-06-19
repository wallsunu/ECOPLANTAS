namespace EcoPlantas.Services.LLM
{
    /// <summary>
    /// Abstracción de un proveedor LLM (Ollama local, OpenAI cloud, etc.).
    /// Permite que el resto de la app (Semantic Kernel) no dependa de un proveedor concreto.
    /// Nunca expone secretos (API keys).
    /// </summary>
    public interface ILlmService
    {
        /// <summary>Nombre del proveedor, ej. "Ollama" u "OpenAI".</summary>
        string Provider { get; }

        /// <summary>Modelo configurado.</summary>
        string Model { get; }

        /// <summary>URL base del proveedor (no es secreto). Puede ser null si no aplica.</summary>
        string? BaseUrl { get; }

        /// <summary>Genera una respuesta para el prompt dado. Lanza <see cref="LlmUnavailableException"/> si falla.</summary>
        Task<string> GenerarRespuestaAsync(string prompt);

        /// <summary>Diagnóstico ligero del proveedor, sin exponer secretos.</summary>
        Task<(bool Disponible, string Mensaje)> ComprobarDisponibilidadAsync();
    }
}
