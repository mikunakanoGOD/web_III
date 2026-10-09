using System.ComponentModel.DataAnnotations;

namespace reforzamiento_primer_parcial.Models;

public class Producto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    public string Nombre { get; set; } = "";
    
    public string Descripcion { get; set; } = "";
    
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
}
