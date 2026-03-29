using Microsoft.AspNetCore.Mvc;
using EcoPlantas.Data;
using EcoPlantas.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace EcoPlantas.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string correo, string password)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Correo == correo && u.Password == password);

            if (usuario != null)
            {
                HttpContext.Session.SetString("Usuario", usuario.Correo);

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.Error = "Correo o contraseña incorrectos";
            return View();
        }
    }
}