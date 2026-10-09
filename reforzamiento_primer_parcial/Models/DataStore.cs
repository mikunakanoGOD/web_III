namespace reforzamiento_primer_parcial.Models;

public static class DataStore
{
    public static List<Cliente> Clientes = new()
    {
        new Cliente { Id = 1, Nombre = "Juan Pérez", Email = "juan@example.com", Telefono = "555-1234" },
        new Cliente { Id = 2, Nombre = "María García", Email = "maria@example.com", Telefono = "555-5678" },
        new Cliente { Id = 3, Nombre = "Carlos López", Email = "carlos@example.com", Telefono = "555-9012" }
    };

    public static List<Producto> Productos = new()
    {
        new Producto { Id = 1, Nombre = "Laptop", Descripcion = "Laptop HP 15\"", Precio = 800.00m, Stock = 9 },
        new Producto { Id = 2, Nombre = "Mouse", Descripcion = "Mouse inalámbrico", Precio = 25.00m, Stock = 48 },
        new Producto { Id = 3, Nombre = "Teclado", Descripcion = "Teclado mecánico", Precio = 75.00m, Stock = 29 },
        new Producto { Id = 4, Nombre = "Monitor", Descripcion = "Monitor 24\" Full HD", Precio = 200.00m, Stock = 14 }
    };

    public static List<Venta> Ventas = new()
    {
        new Venta 
        { 
            Id = 1, 
            ClienteId = 1, 
            Fecha = DateTime.Now.AddDays(-2), 
            Total = 850.00m,
            Detalles = new List<VentaDetalle>
            {
                new VentaDetalle { Id = 1, VentaId = 1, ProductoId = 1, Cantidad = 1, PrecioUnitario = 800.00m },
                new VentaDetalle { Id = 2, VentaId = 1, ProductoId = 2, Cantidad = 2, PrecioUnitario = 25.00m }
            }
        },
        new Venta 
        { 
            Id = 2, 
            ClienteId = 2, 
            Fecha = DateTime.Now.AddDays(-1), 
            Total = 275.00m,
            Detalles = new List<VentaDetalle>
            {
                new VentaDetalle { Id = 3, VentaId = 2, ProductoId = 3, Cantidad = 1, PrecioUnitario = 75.00m },
                new VentaDetalle { Id = 4, VentaId = 2, ProductoId = 4, Cantidad = 1, PrecioUnitario = 200.00m }
            }
        }
    };

    public static int NextClienteId = 4;
    public static int NextProductoId = 5;
    public static int NextVentaId = 3;
    public static int NextVentaDetalleId = 5;
}
