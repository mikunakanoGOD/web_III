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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cita Cita { get; set; } = new Cita();

        public SelectList MascotasSelect { get; set; } = default!;
        public SelectList VeterinariosSelect { get; set; } = default!;
        public SelectList EstadosSelect { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarListasAsync();
            // Por defecto la fecha es ahora
            Cita.FechaHora = DateTime.Now;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return Page();
            }

            _context.Citas.Add(Cita);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task CargarListasAsync()
        {
            var mascotas = await _context.Mascotas
                .Where(m => m.Activo)
                .Include(m => m.Propietario)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            // Mostramos "Nombre (Propietario)" para identificar mejor la mascota
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

            // Dropdown con los valores del enum
            EstadosSelect = new SelectList(
                Enum.GetValues<EstadoCita>().Select(e => new { Value = (int)e, Text = e.ToString() }),
                "Value", "Text");
        }
    }
}
