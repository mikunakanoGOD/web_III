namespace reforzamiento_primer_parcial.Models;

public class VentaViewModel
{
    public int ClienteId { get; set; }
    public List<VentaDetalleItem> Items { get; set; } = new();
}

public class VentaDetalleItem
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}
