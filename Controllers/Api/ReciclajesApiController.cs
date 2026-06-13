using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoPlantas.Data;
using EcoPlantas.Models;
using EcoPlantas.Models.Dto;

namespace EcoPlantas.Controllers.Api
{
    [ApiController]
    [Route("api/reciclajes")]
    public class ReciclajesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReciclajesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reciclajes = await _context.Reciclajes
                .OrderByDescending(r => r.Fecha)
                .Select(r => new ReciclajeDto
                {
                    Id = r.Id,
                    TipoMaterial = r.TipoMaterial,
                    Cantidad = r.Cantidad,
                    Fecha = r.Fecha
                })
                .ToListAsync();

            return Ok(reciclajes);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearReciclajeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reciclaje = new Reciclaje
            {
                TipoMaterial = dto.TipoMaterial,
                Cantidad = dto.Cantidad,
                Fecha = DateTime.UtcNow
            };

            _context.Reciclajes.Add(reciclaje);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new ReciclajeDto
            {
                Id = reciclaje.Id,
                TipoMaterial = reciclaje.TipoMaterial,
                Cantidad = reciclaje.Cantidad,
                Fecha = reciclaje.Fecha
            });
        }
    }
}
