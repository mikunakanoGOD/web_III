using gestion_biblioteca.Models;
using gestion_biblioteca.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace gestion_biblioteca.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Redirige al dashboard correspondiente según el rol del usuario
        public IActionResult Index()
        {
            var usuario = SesionService.ObtenerUsuario(HttpContext.Session);

            if (usuario == null)
                return RedirectToAction("Login", "Account");

            return usuario.Rol switch
            {
                Rol.Administrador => RedirectToAction("Index", "AdminDashboard"),
                Rol.Bibliotecario => RedirectToAction("Index", "BibliotecarioDashboard"),
                Rol.Usuario       => RedirectToAction("Index", "UsuarioDashboard"),
                _                 => RedirectToAction("Login", "Account")
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId  = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = 500
            });
        }
    }
}
