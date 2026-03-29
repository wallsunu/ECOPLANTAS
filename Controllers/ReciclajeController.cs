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

            // obtener usuario logeado desde Session
            var correo = HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(correo))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

            if (usuario != null)
            {
                // sumar puntos
                usuario.Puntos += r.PuntosGanados;
                _context.Update(usuario);
            }

            // guardar reciclaje
            _context.Reciclajes.Add(r);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "Home");
        }
    }
}