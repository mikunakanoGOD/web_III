using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_biblioteca.Models
{
    /// <summary>
    /// Modelo que representa un préstamo de libro a un usuario
    /// </summary>
    public class Prestamo
    {
        [Key]
        public int Id { get; set; }

        // --- Relación con Usuario ---
        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        // --- Relación con Libro ---
        [Required(ErrorMessage = "El libro es obligatorio")]
        [Display(Name = "Libro")]
        public int LibroId { get; set; }

        [ForeignKey("LibroId")]
        public Libro? Libro { get; set; }

        // --- Fechas del préstamo ---
        [Required]
        [Display(Name = "Fecha de Solicitud")]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Aprobación")]
        public DateTime? FechaAprobacion { get; set; }

        [Display(Name = "Fecha de Devolución Esperada")]
        public DateTime? FechaDevolucionEsperada { get; set; }

        [Display(Name = "Fecha de Devolución Real")]
        public DateTime? FechaDevolucionReal { get; set; }

        // --- Estado del préstamo ---
        [Required]
        [Display(Name = "Estado")]
        public EstadoPrestamo Estado { get; set; } = EstadoPrestamo.Pendiente;

        // --- Observaciones opcionales ---
        [StringLength(300, ErrorMessage = "Las observaciones no pueden superar 300 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        // --- Relación con el bibliotecario que gestionó el préstamo ---
        [Display(Name = "Gestionado por (Bibliotecario)")]
        public int? BibliotecarioId { get; set; }

        [ForeignKey("BibliotecarioId")]
        public Usuario? Bibliotecario { get; set; }
    }
}
