using System.ComponentModel.DataAnnotations;

namespace EcoPlantas.Models.Dto
{
    public class ReciclajeDto
    {
        public int Id { get; set; }
        public string TipoMaterial { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class CrearReciclajeDto
    {
        [Required(ErrorMessage = "El tipo de material es obligatorio")]
        public string TipoMaterial { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }
}
