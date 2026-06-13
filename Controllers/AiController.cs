using EcoPlantas.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoPlantas.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly AgentService _agentService;

        public AiController(AgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var respuesta =
                await _agentService
                    .ProcesarPregunta(request.Pregunta);

            return Ok(new
            {
                respuesta
            });
        }
            [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var respuesta = await _agentService
                .ProcesarPregunta("¿Cuánto reciclaje hay registrado?");

            return Ok(respuesta);
        }
    }

    public class ChatRequest
    {
        public string Pregunta { get; set; } = "";
    }
}