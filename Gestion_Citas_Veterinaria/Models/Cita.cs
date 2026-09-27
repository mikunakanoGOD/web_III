using System.ComponentModel.DataAnnotations;

namespace Gestion_Citas_Veterinaria.Models
{
    // Estados posibles de una cita
    public enum EstadoCita
    {
        Pendiente,
        Completada,
        Cancelada
    }

    public class Cita
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La mascota es obligatoria")]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }

        [Required(ErrorMessage = "El veterinario es obligatorio")]
        [Display(Name = "Veterinario")]
        public int VeterinarioId { get; set; }

        [Required(ErrorMessage = "La fecha y hora son obligatorias")]
        [Display(Name = "Fecha y Hora")]
        [DataType(DataType.DateTime)]
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(300)]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;

        [StringLength(500)]
        [Display(Name = "Diagnóstico")]
        public string? Diagnostico { get; set; }

        // Navegación
        public Mascota? Mascota { get; set; }
        public Veterinario? Veterinario { get; set; }
    }
}
