using System.ComponentModel.DataAnnotations;

namespace Gestion_Citas_Veterinaria.Models
{
    public class Mascota
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El propietario es obligatorio")]
        [Display(Name = "Propietario")]
        public int PropietarioId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especie es obligatoria")]
        [StringLength(50)]
        [Display(Name = "Especie")]
        public string Especie { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Raza")]
        public string? Raza { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateOnly? FechaNacimiento { get; set; }

        [Display(Name = "Estado")]
        public bool Activo { get; set; } = true;

        // Navegación
        public Propietario? Propietario { get; set; }
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
