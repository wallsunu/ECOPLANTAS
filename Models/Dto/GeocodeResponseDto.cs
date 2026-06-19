namespace EcoPlantas.Models.Dto
{
    /// <summary>
    /// Respuesta controlada del proceso de geocodificación. Nunca expone la API key.
    /// Cuando la geocodificación falla o no hay key configurada, <see cref="Exito"/>
    /// es false y <see cref="Mensaje"/> explica el motivo (sin lanzar excepciones).
    /// </summary>
    public class GeocodeResponseDto
    {
        public bool Exito { get; set; }
        public string DireccionOriginal { get; set; } = string.Empty;
        public string DireccionFormateada { get; set; } = string.Empty;
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public string Provider { get; set; } = "GoogleMaps";
        public string Mensaje { get; set; } = string.Empty;
    }
}
