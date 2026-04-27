using Microsoft.AspNetCore.Mvc;

namespace EcoPlantas.Controllers
{
    public class AuthController : Controller
    {
        private const string AdminEmail = "admin@ecoplantas.com";
        private const string AdminPassword = "Admin123!";

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Usuario") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (email == AdminEmail && password == AdminPassword)
            {
                HttpContext.Session.SetString("Usuario", email);
                TempData["Exito"] = $"Bienvenido, {email}";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Credenciales incorrectas.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
