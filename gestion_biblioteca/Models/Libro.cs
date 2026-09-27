using System.ComponentModel.DataAnnotations;

namespace gestion_biblioteca.Models
{
    /// <summary>
    /// Modelo que representa un libro del catálogo de la biblioteca
    /// </summary>
    public class Libro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede superar 200 caracteres")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(150, ErrorMessage = "El autor no puede superar 150 caracteres")]
        [Display(Name = "Autor")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ISBN es obligatorio")]
        [StringLength(20, ErrorMessage = "El ISBN no puede superar 20 caracteres")]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "La editorial es obligatoria")]
        [StringLength(150, ErrorMessage = "La editorial no puede superar 150 caracteres")]
        [Display(Name = "Editorial")]
        public string Editorial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año de publicación es obligatorio")]
        [Range(1000, 2100, ErrorMessage = "Ingrese un año válido")]
        [Display(Name = "Año de Publicación")]
        public int AñoPublicacion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(100, ErrorMessage = "La categoría no puede superar 100 caracteres")]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La cantidad total es obligatoria")]
        [Range(1, 9999, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad Total")]
        public int CantidadTotal { get; set; }

        [Display(Name = "Cantidad Disponible")]
        public int CantidadDisponible { get; set; }

        [Display(Name = "Estado")]
        public EstadoLibro Estado { get; set; } = EstadoLibro.Disponible;

        [Display(Name = "Fecha de Ingreso")]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        // Navegación: un libro puede estar en muchos préstamos
        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}
