using EcoPlantas.Models.Dto;
using EcoPlantas.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoPlantas.Controllers.Api
{
    [ApiController]
    [Route("api/mapa")]
    public class MapaController : ControllerBase
    {
        private readonly MapaService _mapaService;

        public MapaController(MapaService mapaService)
        {
            _mapaService = mapaService;
        }

        /// <summary>
        /// Lista de puntos ecológicos (centros de acopio/reciclaje) en Lima y Cieneguilla.
        /// Datos hardcodeados — no usa base de datos — para pintar marcadores en el mapa.
        /// </summary>
        [HttpGet("puntos")]
        public IActionResult GetPuntos()
        {
            var puntos = new List<PuntoEcologicoDto>
            {
                new()
                {
                    Nombre = "Municipalidad de Cieneguilla",
                    Direccion = "Av. Nicolás de Piérola 660, Cieneguilla, Lima",
                    Latitud = -12.1167,
                    Longitud = -76.8167,
                    Descripcion = "Punto principal de acopio y coordinación del recojo municipal."
                },
                new()
                {
                    Nombre = "Punto Ecológico Cieneguilla Centro",
                    Direccion = "Plaza de Armas de Cieneguilla, Lima",
                    Latitud = -12.1185,
                    Longitud = -76.8142,
                    Descripcion = "Contenedores de reciclaje de plástico, papel y vidrio."
                },
                new()
                {
                    Nombre = "Centro de Reciclaje La Molina",
                    Direccion = "Av. Raúl Ferrero, La Molina, Lima",
                    Latitud = -12.0792,
                    Longitud = -76.9447,
                    Descripcion = "Acopio de residuos reciclables y campañas de canje de plantas."
                },
                new()
                {
                    Nombre = "EcoPunto Surco",
                    Direccion = "Av. Caminos del Inca, Santiago de Surco, Lima",
                    Latitud = -12.1357,
                    Longitud = -76.9931,
                    Descripcion = "Estación vecinal de reciclaje aliada a EcoPlantas."
                },
                new()
                {
                    Nombre = "Punto Verde Miraflores",
                    Direccion = "Parque Kennedy, Miraflores, Lima",
                    Latitud = -12.1219,
                    Longitud = -77.0297,
                    Descripcion = "Recolección de residuos electrónicos y reciclables."
                }
            };

            return Ok(puntos);
        }

        /// <summary>
        /// Geocodifica una dirección usando Google Geocoding API (vía MapaService).
        /// Siempre responde 200 con un resultado controlado; si falta la key o
        /// Google responde error, Exito = false con un mensaje explicativo.
        /// </summary>
        [HttpGet("geocode")]
        public async Task<IActionResult> Geocode([FromQuery] string direccion)
        {
            var resultado = await _mapaService.GeocodificarAsync(direccion);
            return Ok(resultado);
        }

        /// <summary>
        /// Informa a la vista si Google Maps está configurado. No expone la API key.
        /// </summary>
        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            return Ok(new
            {
                provider = _mapaService.Provider,
                googleMapsConfigurado = _mapaService.GoogleMapsConfigurado
            });
        }
    }
}
