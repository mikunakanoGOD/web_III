using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestion_biblioteca.Data;
using gestion_biblioteca.Filters;
using gestion_biblioteca.Models;
using gestion_biblioteca.Services;

// Préstamos: Index filtrado por rol; Aprobar/Rechazar solo para Bibliotecario

namespace gestion_biblioteca.Controllers
{
    [RequiereAutenticacion]
    public class PrestamosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrestamosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prestamos — Admin y Biblio ven todos; Usuario ve solo los suyos
        public async Task<IActionResult> Index()
        {
            var sesion = SesionService.ObtenerUsuario(HttpContext.Session)!;

            var query = _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .Include(p => p.Bibliotecario)
                .AsQueryable();

            if (sesion.Rol == Rol.Usuario)
                query = query.Where(p => p.UsuarioId == sesion.Id);

            var prestamos = await query
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();

            ViewBag.UsuarioSesion = sesion;
            return View(prestamos);
        }

        // GET: Prestamos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sesion = SesionService.ObtenerUsuario(HttpContext.Session)!;
            var prestamo = await _context.Prestamos
                .Include(p => p.Bibliotecario)
                .Include(p => p.Libro)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prestamo == null) return NotFound();

            // Usuario solo puede ver sus propios préstamos
            if (sesion.Rol == Rol.Usuario && prestamo.UsuarioId != sesion.Id)
                return RedirectToAction("AccesoDenegado", "Account");

            return View(prestamo);
        }

        // GET: Prestamos/Create
        public IActionResult Create()
        {
            var sesion = SesionService.ObtenerUsuario(HttpContext.Session)!;
            ViewData["LibroId"]   = new SelectList(_context.Libros.Where(l => l.CantidadDisponible > 0), "Id", "Titulo");
            ViewData["UsuarioId"] = sesion.Rol == Rol.Usuario
                ? new SelectList(_context.Usuarios.Where(u => u.Id == sesion.Id), "Id", "Email")
                : new SelectList(_context.Usuarios.Where(u => u.Rol == Rol.Usuario), "Id", "Email");
            return View();
        }

        // POST: Prestamos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UsuarioId,LibroId,Observaciones")] Prestamo prestamo)
        {
            var sesion = SesionService.ObtenerUsuario(HttpContext.Session)!;

            // El usuario solo puede crear préstamos para sí mismo
            if (sesion.Rol == Rol.Usuario)
                prestamo.UsuarioId = sesion.Id;

            prestamo.FechaSolicitud          = DateTime.Now;
            prestamo.FechaDevolucionEsperada = DateTime.Now.AddDays(14);
            prestamo.Estado                  = EstadoPrestamo.Pendiente;

            ModelState.Remove("FechaSolicitud");
            ModelState.Remove("Estado");

            if (ModelState.IsValid)
            {
                _context.Add(prestamo);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Solicitud de préstamo creada correctamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["LibroId"]   = new SelectList(_context.Libros.Where(l => l.CantidadDisponible > 0), "Id", "Titulo", prestamo.LibroId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Where(u => u.Rol == Rol.Usuario), "Id", "Email", prestamo.UsuarioId);
            return View(prestamo);
        }

        // ── Aprobar préstamo (solo Bibliotecario y Admin) ──────────────────────

        // GET: Prestamos/Gestionar/5
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> Gestionar(int? id)
        {
            if (id == null) return NotFound();
            var prestamo = await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prestamo == null) return NotFound();
            return View(prestamo);
        }

        // POST: Prestamos/Aprobar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> Aprobar(int id, string? observaciones)
        {
            var sesion   = SesionService.ObtenerUsuario(HttpContext.Session)!;
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prestamo == null) return NotFound();

            prestamo.Estado          = EstadoPrestamo.Aprobado;
            prestamo.FechaAprobacion = DateTime.Now;
            prestamo.BibliotecarioId = sesion.Id;
            if (!string.IsNullOrWhiteSpace(observaciones))
                prestamo.Observaciones = observaciones;

            // Descontar disponibilidad del libro
            if (prestamo.Libro != null && prestamo.Libro.CantidadDisponible > 0)
            {
                prestamo.Libro.CantidadDisponible--;
                if (prestamo.Libro.CantidadDisponible == 0)
                    prestamo.Libro.Estado = EstadoLibro.Prestado;
            }

            await _context.SaveChangesAsync();
            TempData["Exito"] = "Préstamo aprobado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Prestamos/Rechazar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> Rechazar(int id, string? observaciones)
        {
            var sesion   = SesionService.ObtenerUsuario(HttpContext.Session)!;
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null) return NotFound();

            prestamo.Estado          = EstadoPrestamo.Rechazado;
            prestamo.BibliotecarioId = sesion.Id;
            if (!string.IsNullOrWhiteSpace(observaciones))
                prestamo.Observaciones = observaciones;

            await _context.SaveChangesAsync();
            TempData["Error"] = "Préstamo rechazado.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Prestamos/RegistrarDevolucion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> RegistrarDevolucion(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (prestamo == null) return NotFound();

            prestamo.Estado               = EstadoPrestamo.Devuelto;
            prestamo.FechaDevolucionReal  = DateTime.Now;

            // Restituir disponibilidad del libro
            if (prestamo.Libro != null)
            {
                prestamo.Libro.CantidadDisponible++;
                if (prestamo.Libro.CantidadDisponible > 0)
                    prestamo.Libro.Estado = EstadoLibro.Disponible;
            }

            await _context.SaveChangesAsync();
            TempData["Exito"] = "Devolución registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Prestamos/Delete/5
        [RequiereAutenticacion(Rol.Administrador)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var prestamo = await _context.Prestamos
                .Include(p => p.Bibliotecario).Include(p => p.Libro).Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prestamo == null) return NotFound();
            return View(prestamo);
        }

        // POST: Prestamos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequiereAutenticacion(Rol.Administrador)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo != null) _context.Prestamos.Remove(prestamo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrestamoExists(int id) => _context.Prestamos.Any(e => e.Id == id);
    }
}
