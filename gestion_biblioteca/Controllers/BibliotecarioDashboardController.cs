using gestion_biblioteca.Data;
using gestion_biblioteca.Filters;
using gestion_biblioteca.Models;
using gestion_biblioteca.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblioteca.Controllers
{
    [RequiereAutenticacion(Rol.Bibliotecario)]
    public class BibliotecarioDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BibliotecarioDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = SesionService.ObtenerUsuario(HttpContext.Session);
            ViewBag.Usuario = usuario;

            // Estadísticas para el dashboard del bibliotecario
            ViewBag.TotalLibros         = await _context.Libros.CountAsync();
            ViewBag.LibrosDisponibles   = await _context.Libros
                .CountAsync(l => l.Estado == EstadoLibro.Disponible);
            ViewBag.PrestamosPendientes = await _context.Prestamos
                .CountAsync(p => p.Estado == EstadoPrestamo.Pendiente);
            ViewBag.PrestamosAprobados  = await _context.Prestamos
                .CountAsync(p => p.Estado == EstadoPrestamo.Aprobado);
            ViewBag.Usuario = usuario;

            // Últimos 5 préstamos pendientes
            ViewBag.UltimosPrestamos = await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .Where(p => p.Estado == EstadoPrestamo.Pendiente)
                .OrderByDescending(p => p.FechaSolicitud)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}
