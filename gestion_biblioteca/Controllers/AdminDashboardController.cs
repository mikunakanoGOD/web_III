using gestion_biblioteca.Data;
using gestion_biblioteca.Filters;
using gestion_biblioteca.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblioteca.Controllers
{
    [RequiereAutenticacion(Models.Rol.Administrador)]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = SesionService.ObtenerUsuario(HttpContext.Session);
            ViewBag.Usuario = usuario;

            // Estadísticas para el dashboard del administrador
            ViewBag.TotalUsuarios      = await _context.Usuarios.CountAsync();
            ViewBag.TotalLibros        = await _context.Libros.CountAsync();
            ViewBag.TotalPrestamos     = await _context.Prestamos.CountAsync();
            ViewBag.PrestamosPendientes = await _context.Prestamos
                .CountAsync(p => p.Estado == Models.EstadoPrestamo.Pendiente);
            ViewBag.Usuario = usuario;

            return View();
        }
    }
}
