using Microsoft.AspNetCore.Mvc;
using EcoPlantas.Models.Dto;

namespace EcoPlantas.Controllers.Api
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosApiController : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult Me()
        {
            var email = HttpContext.Session.GetString("UsuarioEmail");
            if (email == null)
                return Unauthorized(new { mensaje = "No hay sesión activa" });

            var idRaw = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol") ?? string.Empty;

            return Ok(new UsuarioMeDto
            {
                UsuarioId = idRaw ?? 0,
                UsuarioEmail = email,
                UsuarioRol = rol
            });
        }
    }
}
