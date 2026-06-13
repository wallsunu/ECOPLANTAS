using System.ComponentModel.DataAnnotations;

namespace EcoPlantas.Models.Dto
{
    public class RecomendarPlantaRequest
    {
        [Required]
        public string TipoMaterial { get; set; } = string.Empty;

        [Range(0.1, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public double CantidadKg { get; set; }

        [Required]
        public string Preferencia { get; set; } = string.Empty;
    }
}
