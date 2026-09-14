using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Citas_Veterinaria.Pages.Propietarios
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
        public Propietario Propietario { get; set; } = new Propietario();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var propietario = await _context.Propietarios.FindAsync(id);
            if (propietario == null)
                return NotFound();

            Propietario = propietario;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Propietario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Propietarios.Any(p => p.Id == Propietario.Id))
                    return NotFound();
                throw;
            }

            return RedirectToPage("Index");
        }
    }
}
