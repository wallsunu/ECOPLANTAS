namespace EcoPlantas.Services
{
    /// <summary>
    /// Excepción controlada que indica que el servicio de IA (Ollama) no está
    /// disponible o no respondió correctamente. El controller la captura para
    /// devolver un mensaje amigable en lugar de un 500 sin controlar.
    /// </summary>
    public class OllamaUnavailableException : Exception
    {
        public OllamaUnavailableException(string message) : base(message) { }
    }
}
