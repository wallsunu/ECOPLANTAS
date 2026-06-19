using EcoPlantas.Services;
using EcoPlantas.Services.LLM;
using Microsoft.AspNetCore.Mvc;

namespace EcoPlantas.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly AgentService _agentService;
        private readonly OllamaService _ollamaService;
        private readonly LlmProviderService _llm;

        public AiController(
            AgentService agentService,
            OllamaService ollamaService,
            LlmProviderService llm)
        {
            _agentService = agentService;
            _ollamaService = ollamaService;
            _llm = llm;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            try
            {
                var respuesta =
                    await _agentService
                        .ProcesarPregunta(request.Pregunta);

                return Ok(new
                {
                    respuesta
                });
            }
            catch (LlmUnavailableException)
            {
                // Respuesta amigable cuando el proveedor LLM (Ollama u OpenAI) no está
                // disponible: caído, 404, sin API key, timeout, etc. Nunca un 500 sin controlar.
                return Ok(new
                {
                    respuesta = "El asistente IA no está disponible por el momento. Verifica la configuración del proveedor de IA."
                });
            }
        }

        /// <summary>
        /// Diagnóstico del proveedor LLM activo (Ollama u OpenAI). No expone la API key.
        /// </summary>
        [HttpGet("llm-health")]
        public async Task<IActionResult> LlmHealth()
        {
            var (disponible, mensaje) = await _llm.ComprobarDisponibilidadAsync();

            return Ok(new
            {
                provider = _llm.Provider,
                model = _llm.Model,
                baseUrl = _llm.BaseUrl,
                disponible,
                mensaje
            });
        }

        /// <summary>
        /// Diagnóstico específico de Ollama (compatibilidad). No expone secretos.
        /// </summary>
        [HttpGet("ollama-health")]
        public async Task<IActionResult> OllamaHealth()
        {
            var (disponible, mensaje) = await _ollamaService.ComprobarDisponibilidadAsync();

            return Ok(new
            {
                baseUrl = _ollamaService.BaseUrl,
                model = _ollamaService.Model,
                disponible,
                mensaje
            });
        }
            [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var respuesta = await _agentService
                .ProcesarPregunta("¿Cuánto reciclaje hay registrado?");

            return Ok(respuesta);
        }

        [HttpGet("semantic-kernel-test")]
        public async Task<IActionResult> SemanticKernelTest()
        {
            var estado = await _agentService.ObtenerEstadoSemanticKernel();
            return Ok(estado);
        }
    }

    public class ChatRequest
    {
        public string Pregunta { get; set; } = "";
    }
}