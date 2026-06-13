namespace EcoPlantas.Models.Dto
{
    public class UsuarioMeDto
    {
        public int UsuarioId { get; set; }
        public string UsuarioEmail { get; set; } = string.Empty;
        public string UsuarioRol { get; set; } = string.Empty;
    }
}
