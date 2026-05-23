using Microsoft.AspNetCore.Mvc;
using EcoPlantas.Data;
using EcoPlantas.Models;
using EcoPlantas.Models.Dto;

namespace EcoPlantas.Controllers.Api
{
    [ApiController]
    [Route("api/contacto")]
    public class ContactoApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearContactoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mensaje = new ContactoMensaje
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Mensaje = dto.Mensaje,
                FechaRegistro = DateTime.UtcNow
            };

            _context.ContactoMensajes.Add(mensaje);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Mensaje enviado correctamente", id = mensaje.Id });
        }
    }
}
