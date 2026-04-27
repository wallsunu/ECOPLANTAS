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
            return View(new ContactoMensaje());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactoMensaje contacto)
        {
            if (!ModelState.IsValid)
            {
                return View(contacto);
            }

            contacto.FechaRegistro = DateTime.UtcNow;
            _context.ContactoMensajes.Add(contacto);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Mensaje enviado correctamente.";
            return RedirectToAction(nameof(Index));
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
