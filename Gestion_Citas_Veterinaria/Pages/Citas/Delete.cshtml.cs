using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Citas_Veterinaria.Pages.Citas
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cita Cita { get; set; } = new Cita();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.Veterinario)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
                return NotFound();

            Cita = cita;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cita = await _context.Citas.FindAsync(Cita.Id);
            if (cita != null)
            {
                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
