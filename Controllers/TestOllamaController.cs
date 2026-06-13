using Microsoft.AspNetCore.Mvc;
using EcoPlantas.Services;

namespace EcoPlantas.Controllers
{
    [ApiController]
    [Route("api/test-ollama")]
    public class TestOllamaController : ControllerBase
    {
        private readonly OllamaService _ollamaService;

        public TestOllamaController(OllamaService ollamaService)
        {
            _ollamaService = ollamaService;
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            var respuesta =
                await _ollamaService.GenerarRespuestaAsync(
                    "Responde únicamente: EcoPlantas conectado correctamente");

            return Ok(respuesta);
        }
    }
}