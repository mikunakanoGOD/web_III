using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Citas_Veterinaria.Pages.Mascotas
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Mascota> Mascotas { get; set; } = new List<Mascota>();

        public async Task OnGetAsync()
        {
            // Incluimos el propietario para mostrarlo en la tabla
            Mascotas = await _context.Mascotas
                .Include(m => m.Propietario)
                .ToListAsync();
        }
    }
}
