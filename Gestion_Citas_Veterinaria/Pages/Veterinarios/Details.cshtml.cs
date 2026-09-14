using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gestion_Citas_Veterinaria.Pages.Veterinarios
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Veterinario Veterinario { get; set; } = new Veterinario();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var veterinario = await _context.Veterinarios.FindAsync(id);
            if (veterinario == null)
                return NotFound();

            Veterinario = veterinario;
            return Page();
        }
    }
}
