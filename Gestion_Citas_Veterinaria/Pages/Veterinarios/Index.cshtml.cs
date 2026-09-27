using Gestion_Citas_Veterinaria.Data;
using Gestion_Citas_Veterinaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Citas_Veterinaria.Pages.Veterinarios
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Veterinario> Veterinarios { get; set; } = new List<Veterinario>();

        public async Task OnGetAsync()
        {
            Veterinarios = await _context.Veterinarios.ToListAsync();
        }
    }
}
