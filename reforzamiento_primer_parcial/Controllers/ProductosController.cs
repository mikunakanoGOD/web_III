using Microsoft.AspNetCore.Mvc;
using reforzamiento_primer_parcial.Models;

namespace reforzamiento_primer_parcial.Controllers;

public class ProductosController : Controller
{
    // GET: Productos
    public IActionResult Index()
    {
        return View(DataStore.Productos);
    }

    // GET: Productos/Details/5
    public IActionResult Details(int id)
    {
        var producto = DataStore.Productos.FirstOrDefault(p => p.Id == id);
        if (producto == null)
        {
            return NotFound();
        }
        return View(producto);
    }

    // GET: Productos/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Producto producto)
    {
        if (ModelState.IsValid)
        {
            producto.Id = DataStore.NextProductoId++;
            DataStore.Productos.Add(producto);
            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }

    // GET: Productos/Edit/5
    public IActionResult Edit(int id)
    {
        var producto = DataStore.Productos.FirstOrDefault(p => p.Id == id);
        if (producto == null)
        {
            return NotFound();
        }
        return View(producto);
    }

    // POST: Productos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Producto producto)
    {
        if (id != producto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingProducto = DataStore.Productos.FirstOrDefault(p => p.Id == id);
            if (existingProducto == null)
            {
                return NotFound();
            }

            existingProducto.Nombre = producto.Nombre;
            existingProducto.Descripcion = producto.Descripcion;
            existingProducto.Precio = producto.Precio;
            existingProducto.Stock = producto.Stock;

            return RedirectToAction(nameof(Index));
        }
        return View(producto);
    }

    // GET: Productos/Delete/5
    public IActionResult Delete(int id)
    {
        var producto = DataStore.Productos.FirstOrDefault(p => p.Id == id);
        if (producto == null)
        {
            return NotFound();
        }
        return View(producto);
    }

    // POST: Productos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var producto = DataStore.Productos.FirstOrDefault(p => p.Id == id);
        if (producto != null)
        {
            DataStore.Productos.Remove(producto);
        }
        return RedirectToAction(nameof(Index));
    }
}
