using gestion_biblioteca.Models;
using gestion_biblioteca.Services;
using gestion_biblioteca.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace gestion_biblioteca.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAutenticacionService _autenticacion;

        public AccountController(IAutenticacionService autenticacion)
        {
            _autenticacion = autenticacion;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Si ya está autenticado, redirigir al dashboard
            if (SesionService.EstaAutenticado(HttpContext.Session))
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _autenticacion.ValidarCredencialesAsync(model.Email, model.Password);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View(model);
            }

            // Guardar datos del usuario en sesión
            SesionService.GuardarUsuario(HttpContext.Session, usuario);

            // Redirigir según rol
            return RedirectSegunRol(usuario.Rol, returnUrl);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            SesionService.CerrarSesion(HttpContext.Session);
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/AccesoDenegado
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        // --- Helpers privados ---

        private IActionResult RedirectSegunRol(Rol rol, string? returnUrl)
        {
            // Si hay una URL de retorno válida, usarla
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Redirigir al dashboard según el rol
            return rol switch
            {
                Rol.Administrador => RedirectToAction("Index", "AdminDashboard"),
                Rol.Bibliotecario => RedirectToAction("Index", "BibliotecarioDashboard"),
                Rol.Usuario       => RedirectToAction("Index", "UsuarioDashboard"),
                _                 => RedirectToAction("Index", "Home")
            };
        }
    }
}
