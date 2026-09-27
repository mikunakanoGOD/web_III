using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Citas_Veterinaria.Pages.Citas
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cita> Citas { get; set; } = new List<Cita>();

        public async Task OnGetAsync()
        {
            Citas = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.Veterinario)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync();
        }
    }
}
