using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcoPlantas.Models;
using EcoPlantas.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace EcoPlantas.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Dashboard()
    {
        var correo = HttpContext.Session.GetString("Usuario");

        if (string.IsNullOrEmpty(correo))
        {
            return RedirectToAction("Login", "Account");
        }

        var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

        if (usuario == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(usuario);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult Ranking()
    {
        var usuarios = _context.Usuarios
            .OrderByDescending(u => u.Puntos)
            .ToList();

        return View(usuarios);
    }
}
