using Microsoft.AspNetCore.Mvc;
using reforzamiento_primer_parcial.Models;

namespace reforzamiento_primer_parcial.Controllers;

public class ClientesController : Controller
{
    // GET: Clientes
    public IActionResult Index()
    {
        return View(DataStore.Clientes);
    }

    // GET: Clientes/Details/5
    public IActionResult Details(int id)
    {
        var cliente = DataStore.Clientes.FirstOrDefault(c => c.Id == id);
        if (cliente == null)
        {
            return NotFound();
        }
        return View(cliente);
    }

    // GET: Clientes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Clientes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            cliente.Id = DataStore.NextClienteId++;
            DataStore.Clientes.Add(cliente);
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }

    // GET: Clientes/Edit/5
    public IActionResult Edit(int id)
    {
        var cliente = DataStore.Clientes.FirstOrDefault(c => c.Id == id);
        if (cliente == null)
        {
            return NotFound();
        }
        return View(cliente);
    }

    // POST: Clientes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Cliente cliente)
    {
        if (id != cliente.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingCliente = DataStore.Clientes.FirstOrDefault(c => c.Id == id);
            if (existingCliente == null)
            {
                return NotFound();
            }

            existingCliente.Nombre = cliente.Nombre;
            existingCliente.Email = cliente.Email;
            existingCliente.Telefono = cliente.Telefono;

            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }

    // GET: Clientes/Delete/5
    public IActionResult Delete(int id)
    {
        var cliente = DataStore.Clientes.FirstOrDefault(c => c.Id == id);
        if (cliente == null)
        {
            return NotFound();
        }
        return View(cliente);
    }

    // POST: Clientes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var cliente = DataStore.Clientes.FirstOrDefault(c => c.Id == id);
        if (cliente != null)
        {
            DataStore.Clientes.Remove(cliente);
        }
        return RedirectToAction(nameof(Index));
    }
}
