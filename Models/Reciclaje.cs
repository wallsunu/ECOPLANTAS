using System;
using System.ComponentModel.DataAnnotations;

namespace EcoPlantas.Models
{
    public class Reciclaje
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de material es obligatorio")]
        [Display(Name = "Tipo de material")]
        public string TipoMaterial { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad (kg)")]
        public int Cantidad { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
