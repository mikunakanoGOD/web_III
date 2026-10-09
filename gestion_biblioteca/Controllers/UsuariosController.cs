using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestion_biblioteca.Data;
using gestion_biblioteca.Filters;
using gestion_biblioteca.Models;
using gestion_biblioteca.Services;

// Usuarios: solo el Administrador puede gestionarlos

namespace gestion_biblioteca.Controllers
{
    [RequiereAutenticacion(Rol.Administrador)]
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAutenticacionService _autenticacion;

        public UsuariosController(ApplicationDbContext context, IAutenticacionService autenticacion)
        {
            _context        = context;
            _autenticacion  = autenticacion;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.OrderBy(u => u.Rol).ThenBy(u => u.Apellido).ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create() => View();

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Nombre,Apellido,Email,PasswordHash,Rol,Activo")] Usuario usuario)
        {
            // Verificar email único
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
            {
                ModelState.AddModelError("Email", "Ya existe un usuario con este correo electrónico.");
                return View(usuario);
            }

            if (ModelState.IsValid)
            {
                // Hashear contraseña antes de guardar
                usuario.PasswordHash  = _autenticacion.HashearPassword(usuario.PasswordHash);
                usuario.FechaRegistro = DateTime.Now;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Exito"] = $"Usuario {usuario.NombreCompleto} creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Nombre,Apellido,Email,Rol,Activo,FechaRegistro,PasswordHash")] Usuario usuario,
            string? nuevaPassword)
        {
            if (id != usuario.Id) return NotFound();

            // Verificar email único (excluyendo el propio)
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email && u.Id != id))
            {
                ModelState.AddModelError("Email", "Ya existe un usuario con este correo electrónico.");
                return View(usuario);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Solo cambiar contraseña si se proporcionó una nueva
                    if (!string.IsNullOrWhiteSpace(nuevaPassword))
                        usuario.PasswordHash = _autenticacion.HashearPassword(nuevaPassword);

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                { if (!UsuarioExists(usuario.Id)) return NotFound(); else throw; }

                TempData["Exito"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                // Soft delete: desactivar en lugar de eliminar
                usuario.Activo = false;
                _context.Update(usuario);
            }
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Usuario desactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id) => _context.Usuarios.Any(e => e.Id == id);
    }
}
