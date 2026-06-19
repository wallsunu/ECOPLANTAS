using EcoPlantas.Services.LLM;

namespace EcoPlantas.Services
{
    /// <summary>
    /// Excepción controlada específica de Ollama. Hereda de <see cref="LlmUnavailableException"/>
    /// para que el manejo genérico de proveedores LLM también la capture.
    /// </summary>
    public class OllamaUnavailableException : LlmUnavailableException
    {
        public OllamaUnavailableException(string message) : base(message) { }
    }
}
