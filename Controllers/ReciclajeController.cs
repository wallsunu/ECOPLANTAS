using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoPlantas.Data;
using EcoPlantas.Models;

namespace EcoPlantas.Controllers
{
    public class ReciclajeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReciclajeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UsuarioEmail") == null)
                return RedirectToAction("Login", "Auth");

            var reciclajes = await _context.Reciclajes
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            ViewBag.TotalKg = reciclajes.Sum(r => r.Cantidad);
            return View(reciclajes);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            if (HttpContext.Session.GetString("UsuarioEmail") == null)
                return RedirectToAction("Login", "Auth");

            return View(new Reciclaje { Fecha = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Reciclaje reciclaje)
        {
            if (HttpContext.Session.GetString("UsuarioEmail") == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(reciclaje);

            reciclaje.Fecha = DateTime.UtcNow;
            _context.Reciclajes.Add(reciclaje);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Se registraron {reciclaje.Cantidad} kg de {reciclaje.TipoMaterial} correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            if (HttpContext.Session.GetString("UsuarioEmail") == null)
                return RedirectToAction("Login", "Auth");

            var reciclaje = await _context.Reciclajes.FindAsync(id);
            if (reciclaje == null) return NotFound();
            return View(reciclaje);
        }
    }
}
