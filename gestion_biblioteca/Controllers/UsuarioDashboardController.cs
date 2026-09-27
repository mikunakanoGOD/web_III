using gestion_biblioteca.Data;
using gestion_biblioteca.Filters;
using gestion_biblioteca.Models;
using gestion_biblioteca.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblioteca.Controllers
{
    [RequiereAutenticacion(Rol.Usuario)]
    public class UsuarioDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuarioDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = SesionService.ObtenerUsuario(HttpContext.Session)!;
            ViewBag.Usuario = usuario;

            // Mis préstamos activos
            ViewBag.MisPrestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Where(p => p.UsuarioId == usuario.Id &&
                            (p.Estado == EstadoPrestamo.Pendiente ||
                             p.Estado == EstadoPrestamo.Aprobado))
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();

            // Total de libros disponibles en el catálogo
            ViewBag.LibrosDisponibles = await _context.Libros
                .CountAsync(l => l.Estado == EstadoLibro.Disponible);

            // Total de mis préstamos
            ViewBag.TotalMisPrestamos = await _context.Prestamos
                .CountAsync(p => p.UsuarioId == usuario.Id);

            return View();
        }
    }
}
