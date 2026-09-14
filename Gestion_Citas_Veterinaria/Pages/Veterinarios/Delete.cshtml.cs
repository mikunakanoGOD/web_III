using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gestion_Citas_Veterinaria.Pages.Veterinarios
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
        public Veterinario Veterinario { get; set; } = new Veterinario();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var veterinario = await _context.Veterinarios.FindAsync(id);
            if (veterinario == null)
                return NotFound();

            Veterinario = veterinario;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var veterinario = await _context.Veterinarios.FindAsync(Veterinario.Id);
            if (veterinario != null)
            {
                _context.Veterinarios.Remove(veterinario);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
