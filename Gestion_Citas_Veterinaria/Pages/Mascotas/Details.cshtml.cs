using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Citas_Veterinaria.Pages.Mascotas
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Mascota Mascota { get; set; } = new Mascota();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var mascota = await _context.Mascotas
                .Include(m => m.Propietario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mascota == null)
                return NotFound();

            Mascota = mascota;
            return Page();
        }
    }
}
