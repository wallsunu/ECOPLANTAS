using System.ComponentModel.DataAnnotations;

namespace EcoPlantas.Models.Dto
{
    public class CrearContactoDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje es obligatorio")]
        public string Mensaje { get; set; } = string.Empty;
    }
}
