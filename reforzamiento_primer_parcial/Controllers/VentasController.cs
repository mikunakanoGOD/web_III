using Microsoft.AspNetCore.Mvc;
using reforzamiento_primer_parcial.Models;

namespace reforzamiento_primer_parcial.Controllers;

public class VentasController : Controller
{
    // GET: Ventas
    public IActionResult Index()
    {
        // Resolve Cliente navigation property
        foreach (var venta in DataStore.Ventas)
        {
            venta.Cliente = DataStore.Clientes.FirstOrDefault(c => c.Id == venta.ClienteId);
        }
        return View(DataStore.Ventas);
    }

    // GET: Ventas/Details/5
    public IActionResult Details(int id)
    {
        var venta = DataStore.Ventas.FirstOrDefault(v => v.Id == id);
        if (venta == null)
        {
            return NotFound();
        }

        // Resolve navigation properties
        venta.Cliente = DataStore.Clientes.FirstOrDefault(c => c.Id == venta.ClienteId);
        foreach (var detalle in venta.Detalles)
        {
            detalle.Producto = DataStore.Productos.FirstOrDefault(p => p.Id == detalle.ProductoId);
        }

        return View(venta);
    }

    // GET: Ventas/Create
    public IActionResult Create()
    {
        ViewBag.Clientes = DataStore.Clientes;
        ViewBag.Productos = DataStore.Productos;
        return View();
    }

    // POST: Ventas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(VentaViewModel model)
    {
        // NOTA: Este código no maneja concurrencia. En un entorno de producción con múltiples
        // usuarios simultáneos, sería necesario implementar lock() o transacciones para evitar
        // condiciones de carrera en la validación y deducción de stock.
        
        // Validate at least one item
        if (model.Items == null || model.Items.Count == 0 || model.Items.All(i => i.ProductoId == 0))
        {
            ModelState.AddModelError("", "Debe agregar al menos un producto a la venta");
            ViewBag.Clientes = DataStore.Clientes;
            ViewBag.Productos = DataStore.Productos;
            return View(model);
        }

        // Filter out empty items
        var validItems = model.Items.Where(i => i.ProductoId > 0 && i.Cantidad > 0).ToList();
        
        if (validItems.Count == 0)
        {
            ModelState.AddModelError("", "Debe agregar al menos un producto válido a la venta");
            ViewBag.Clientes = DataStore.Clientes;
            ViewBag.Productos = DataStore.Productos;
            return View(model);
        }

        // Validate stock for each item
        foreach (var item in validItems)
        {
            var producto = DataStore.Productos.FirstOrDefault(p => p.Id == item.ProductoId);
            if (producto == null)
            {
                ModelState.AddModelError("", $"Producto con ID {item.ProductoId} no encontrado");
                ViewBag.Clientes = DataStore.Clientes;
                ViewBag.Productos = DataStore.Productos;
                return View(model);
            }

            if (producto.Stock < item.Cantidad)
            {
                ModelState.AddModelError("", $"Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}");
                ViewBag.Clientes = DataStore.Clientes;
                ViewBag.Productos = DataStore.Productos;
                return View(model);
            }
        }

        // Create venta
        var venta = new Venta
        {
            Id = DataStore.NextVentaId++,
            ClienteId = model.ClienteId,
            Fecha = DateTime.Now,
            Total = 0
        };

        // Create detalles and calculate total
        decimal total = 0;
        foreach (var item in validItems)
        {
            var producto = DataStore.Productos.First(p => p.Id == item.ProductoId);
            
            var detalle = new VentaDetalle
            {
                Id = DataStore.NextVentaDetalleId++,
                VentaId = venta.Id,
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            };

            venta.Detalles.Add(detalle);
            total += detalle.Subtotal;

            // Deduct stock
            producto.Stock -= item.Cantidad;
        }

        venta.Total = total;
        DataStore.Ventas.Add(venta);

        return RedirectToAction(nameof(Index));
    }
}
