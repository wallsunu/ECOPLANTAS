using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Reciclaje r)
        {
            // calcular puntos
            r.PuntosGanados = r.Cantidad * 10;

            _context.Reciclajes.Add(r);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "Home");
        }
    }
}