namespace EcoPlantas.Models.Dto
{
    /// <summary>
    /// Representa un punto ecológico (centro de reciclaje / acopio) que se
    /// muestra como marcador en el mapa de Google Maps de la página principal.
    /// </summary>
    public class PuntoEcologicoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
