using Microsoft.AspNetCore.Mvc;
using EcoPlantas.Data;
using EcoPlantas.Models;

namespace EcoPlantas.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactoMensaje mensaje)
        {
            if (ModelState.IsValid)
            {
                mensaje.FechaRegistro = DateTime.Now;
                _context.ContactoMensajes.Add(mensaje);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "¡Mensaje enviado con éxito! Nos pondremos en contacto contigo pronto.";
                return RedirectToAction(nameof(Index));
            }
            return View(mensaje);
        }

        [HttpGet]
        public IActionResult Mensajes()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Login", "Auth");

            var mensajes = _context.ContactoMensajes.OrderByDescending(m => m.FechaRegistro).ToList();
            return View(mensajes);
        }
    }
}
