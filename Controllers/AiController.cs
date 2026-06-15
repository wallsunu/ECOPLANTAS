using EcoPlantas.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoPlantas.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly AgentService _agentService;
        private readonly OllamaService _ollamaService;

        public AiController(AgentService agentService, OllamaService ollamaService)
        {
            _agentService = agentService;
            _ollamaService = ollamaService;
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
            catch (OllamaUnavailableException)
            {
                // Respuesta amigable cuando Ollama no está disponible (caído, 404, timeout, etc.).
                return Ok(new
                {
                    respuesta = "El asistente IA no está disponible por el momento. Verifica la configuración de Ollama."
                });
            }
        }

        /// <summary>
        /// Diagnóstico de Ollama. No expone secretos (la URL base no es un secreto).
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