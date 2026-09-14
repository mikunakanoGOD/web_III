using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gestion_Citas_Veterinaria.Pages.Propietarios
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Propietario Propietario { get; set; } = new Propietario();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var propietario = await _context.Propietarios.FindAsync(id);
            if (propietario == null)
                return NotFound();

            Propietario = propietario;
            return Page();
        }
    }
}
