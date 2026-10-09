using System.ComponentModel.DataAnnotations;

namespace reforzamiento_primer_parcial.Models;

public class Cliente
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    public string Nombre { get; set; } = "";
    
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = "";
    
    public string Telefono { get; set; } = "";
}
