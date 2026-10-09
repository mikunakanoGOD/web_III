namespace reforzamiento_primer_parcial.Models;

public class Venta
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public Cliente? Cliente { get; set; }
    public List<VentaDetalle> Detalles { get; set; } = new();
}
