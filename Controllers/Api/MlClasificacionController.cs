using Microsoft.AspNetCore.Mvc;
using EcoPlantas.Services.ML;
using System.ComponentModel.DataAnnotations;

namespace EcoPlantas.Controllers.Api
{
    [ApiController]
    [Route("api/ml")]
    public class MlClasificacionController : ControllerBase
    {
        private readonly ClasificacionReciclajeService _clasificacion;

        public MlClasificacionController(ClasificacionReciclajeService clasificacion)
        {
            _clasificacion = clasificacion;
        }

        [HttpPost("clasificar-reciclaje")]
        public IActionResult ClasificarReciclaje([FromBody] ClasificarReciclajeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var prediccion = _clasificacion.Clasificar(request.TipoMaterial, (float)request.CantidadKg);

            var mensaje = prediccion.NivelImpacto switch
            {
                "Alto"  => $"Reciclaje de {request.CantidadKg} kg de {request.TipoMaterial} tiene un impacto ambiental ALTO. ¡Excelente aporte a Cieneguilla!",
                "Medio" => $"Reciclaje de {request.CantidadKg} kg de {request.TipoMaterial} tiene un impacto ambiental MEDIO. ¡Sigue sumando!",
                _       => $"Reciclaje de {request.CantidadKg} kg de {request.TipoMaterial} tiene un impacto ambiental BAJO. Cada kilo cuenta.",
            };

            return Ok(new
            {
                tipoMaterial = request.TipoMaterial,
                cantidadKg   = request.CantidadKg,
                nivelImpacto = prediccion.NivelImpacto,
                mensaje
            });
        }
    }

    public class ClasificarReciclajeRequest
    {
        [Required]
        public string TipoMaterial { get; set; } = string.Empty;

        [Range(0.1, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public double CantidadKg { get; set; }
    }
}
