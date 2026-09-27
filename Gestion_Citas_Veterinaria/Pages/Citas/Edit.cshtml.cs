using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Citas_Veterinaria.Pages.Citas
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cita Cita { get; set; } = new Cita();

        public SelectList MascotasSelect { get; set; } = default!;
        public SelectList VeterinariosSelect { get; set; } = default!;
        public SelectList EstadosSelect { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
                return NotFound();

            Cita = cita;
            await CargarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return Page();
            }

            _context.Attach(Cita).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Citas.Any(c => c.Id == Cita.Id))
                    return NotFound();
                throw;
            }

            return RedirectToPage("Index");
        }

        private async Task CargarListasAsync()
        {
            var mascotas = await _context.Mascotas
                .Where(m => m.Activo)
                .Include(m => m.Propietario)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            MascotasSelect = new SelectList(
                mascotas.Select(m => new { m.Id, Texto = $"{m.Nombre} ({m.Propietario?.Apellidos})" }),
                "Id", "Texto");

            var veterinarios = await _context.Veterinarios
                .Where(v => v.Activo)
                .OrderBy(v => v.Apellidos)
                .ToListAsync();

            VeterinariosSelect = new SelectList(
                veterinarios.Select(v => new { v.Id, Texto = $"{v.Nombre} {v.Apellidos}" }),
                "Id", "Texto");

            EstadosSelect = new SelectList(
                Enum.GetValues<EstadoCita>().Select(e => new { Value = (int)e, Text = e.ToString() }),
                "Value", "Text");
        }
    }
}
