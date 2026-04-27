using System.ComponentModel.DataAnnotations;

namespace EcoPlantas.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = "Admin";

        public bool Activo { get; set; } = true;
    }
}
