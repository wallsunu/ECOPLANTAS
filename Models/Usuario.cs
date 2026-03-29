using System.ComponentModel.DataAnnotations;

namespace EcoPlantas.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Correo { get; set; }

        [Required]
        public string Password { get; set; }

        public int Puntos { get; set; } = 0;
    }
}