namespace EcoPlantas.Services.LLM
{
    /// <summary>
    /// Excepción controlada que indica que el proveedor LLM configurado no está
    /// disponible o no respondió correctamente. El controller la captura para
    /// devolver un mensaje amigable en lugar de un 500 sin controlar.
    /// </summary>
    public class LlmUnavailableException : Exception
    {
        public LlmUnavailableException(string message) : base(message) { }
    }
}
