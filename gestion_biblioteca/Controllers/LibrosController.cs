using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestion_biblioteca.Data;
using gestion_biblioteca.Filters;
using gestion_biblioteca.Models;
using gestion_biblioteca.Services;

// Libros: CRUD para Admin y Bibliotecario; Catálogo de solo lectura para Usuario

namespace gestion_biblioteca.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─── CATÁLOGO (todos los roles autenticados) ───────────────────────────

        // GET: Libros/Catalogo — vista de solo lectura para todos los roles
        [RequiereAutenticacion]
        public async Task<IActionResult> Catalogo(string? buscar, string? categoria)
        {
            var usuario = SesionService.ObtenerUsuario(HttpContext.Session);
            ViewBag.UsuarioSesion = usuario;

            var librosQuery = _context.Libros.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
                librosQuery = librosQuery.Where(l =>
                    l.Titulo.Contains(buscar) || l.Autor.Contains(buscar));

            if (!string.IsNullOrWhiteSpace(categoria))
                librosQuery = librosQuery.Where(l => l.Categoria == categoria);

            ViewBag.Buscar    = buscar;
            ViewBag.Categoria = categoria;
            ViewBag.Categorias = await _context.Libros
                .Select(l => l.Categoria).Distinct().OrderBy(c => c).ToListAsync();

            return View(await librosQuery.OrderBy(l => l.Titulo).ToListAsync());
        }

        // POST: Libros/SolicitarPrestamo — solo rol Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiereAutenticacion(Rol.Usuario)]
        public async Task<IActionResult> SolicitarPrestamo(int libroId)
        {
            var usuario = SesionService.ObtenerUsuario(HttpContext.Session)!;

            var libro = await _context.Libros.FindAsync(libroId);
            if (libro == null || libro.CantidadDisponible <= 0)
            {
                TempData["Error"] = "El libro no está disponible en este momento.";
                return RedirectToAction(nameof(Catalogo));
            }

            // Verificar que el usuario no tenga ya un préstamo activo del mismo libro
            bool yaEnPrestamo = await _context.Prestamos.AnyAsync(p =>
                p.UsuarioId == usuario.Id && p.LibroId == libroId &&
                (p.Estado == EstadoPrestamo.Pendiente || p.Estado == EstadoPrestamo.Aprobado));

            if (yaEnPrestamo)
            {
                TempData["Error"] = "Ya tienes un préstamo activo para este libro.";
                return RedirectToAction(nameof(Catalogo));
            }

            var prestamo = new Prestamo
            {
                UsuarioId              = usuario.Id,
                LibroId                = libroId,
                FechaSolicitud         = DateTime.Now,
                FechaDevolucionEsperada = DateTime.Now.AddDays(14),
                Estado                 = EstadoPrestamo.Pendiente
            };

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Solicitud de préstamo para \"{libro.Titulo}\" enviada correctamente.";
            return RedirectToAction(nameof(Catalogo));
        }

        // ─── CRUD (Admin y Bibliotecario) ──────────────────────────────────────

        // GET: Libros
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Libros.OrderBy(l => l.Titulo).ToListAsync());
        }

        // GET: Libros/Details/5
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var libro = await _context.Libros.FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        // GET: Libros/Create
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public IActionResult Create() => View();

        // POST: Libros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> Create(
            [Bind("Titulo,Autor,ISBN,Editorial,AñoPublicacion,Categoria,Descripcion,CantidadTotal,Estado")]
            Libro libro)
        {
            if (ModelState.IsValid)
            {
                libro.CantidadDisponible = libro.CantidadTotal;
                libro.FechaIngreso       = DateTime.Now;
                _context.Add(libro);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Libro agregado al catálogo correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(libro);
        }

        // GET: Libros/Edit/5
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Titulo,Autor,ISBN,Editorial,AñoPublicacion,Categoria,Descripcion,CantidadTotal,CantidadDisponible,Estado,FechaIngreso")]
            Libro libro)
        {
            if (id != libro.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(libro); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException)
                { if (!LibroExists(libro.Id)) return NotFound(); else throw; }
                TempData["Exito"] = "Libro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(libro);
        }

        // GET: Libros/Delete/5
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var libro = await _context.Libros.FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null) _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Libro eliminado del catálogo.";
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id) => _context.Libros.Any(e => e.Id == id);
    }
}
