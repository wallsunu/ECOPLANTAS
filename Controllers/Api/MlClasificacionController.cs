using Microsoft.AspNetCore.Mvc;
using EcoPlantas.Services.ML;
using EcoPlantas.Models.Dto;
using System.ComponentModel.DataAnnotations;

namespace EcoPlantas.Controllers.Api
{
    [ApiController]
    [Route("api/ml")]
    public class MlClasificacionController : ControllerBase
    {
        private readonly ClasificacionReciclajeService _clasificacion;
        private readonly RecomendacionPlantaService _recomendacion;

        public MlClasificacionController(
            ClasificacionReciclajeService clasificacion,
            RecomendacionPlantaService recomendacion)
        {
            _clasificacion = clasificacion;
            _recomendacion = recomendacion;
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
        [HttpPost("recomendar-planta")]
        public IActionResult RecomendarPlanta([FromBody] RecomendarPlantaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (planta, motivo) = _recomendacion.Recomendar(
                request.TipoMaterial,
                (float)request.CantidadKg,
                request.Preferencia);

            return Ok(new
            {
                tipoMaterial     = request.TipoMaterial,
                cantidadKg       = request.CantidadKg,
                preferencia      = request.Preferencia,
                plantaRecomendada = planta,
                motivo
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
