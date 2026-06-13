using Microsoft.AspNetCore.Mvc;
using EcoPlantas.Data;
using EcoPlantas.Models;

namespace EcoPlantas.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Login ────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UsuarioEmail") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario == null || !usuario.Activo || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                ViewBag.Error = "Credenciales incorrectas.";
                return View();
            }

            IniciarSesion(usuario);
            TempData["Exito"] = $"Bienvenido, {usuario.Email}";
            return RedirectToAction("Index", "Home");
        }

        // ── Register ─────────────────────────────────────────────

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UsuarioEmail") != null)
                return RedirectToAction("Index", "Home");
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_context.Usuarios.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Este email ya está registrado.");
                return View(model);
            }

            var usuario = new Usuario
            {
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Rol = "Usuario",
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            IniciarSesion(usuario);
            TempData["Exito"] = $"Cuenta creada. ¡Bienvenido, {usuario.Email}!";
            return RedirectToAction("Index", "Reciclaje");
        }

        // ── Logout ───────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ── Helpers ──────────────────────────────────────────────

        private void IniciarSesion(Usuario usuario)
        {
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol);
        }
    }
}
