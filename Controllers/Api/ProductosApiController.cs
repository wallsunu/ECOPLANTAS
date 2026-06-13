using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoPlantas.Data;
using EcoPlantas.Models.Dto;

namespace EcoPlantas.Controllers.Api
{
    [ApiController]
    [Route("api/productos")]
    public class ProductosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _context.Productos
                .Where(p => p.Disponible)
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    ImagenUrl = p.ImagenUrl,
                    Disponible = p.Disponible
                })
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _context.Productos.FindAsync(id);
            if (p == null) return NotFound(new { mensaje = "Producto no encontrado" });

            return Ok(new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Categoria = p.Categoria,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                ImagenUrl = p.ImagenUrl,
                Disponible = p.Disponible
            });
        }
    }
}
