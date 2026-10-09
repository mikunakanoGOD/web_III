# Modular sales-management system

A simple ASP.NET Core MVC application for managing customers, products, and multi-item sales using in-memory storage. The implementation follows the modular architecture requested (separate controllers and views for each entity type), includes all required CRUD operations, handles stock deduction on sale creation, validates at-least-one-product constraint, and builds successfully. The three key gaps: sale edits and cancellations are not implemented (sales are write-once), stock deduction on pre-seeded sample sales creates an inconsistency between declared and actual inventory, and concurrent access to the shared DataStore will corrupt state.

**Watch for:** Pre-seeded sample sales (confirmed) — the DataStore initializes two historical sales but never deducts their quantities from product stock, so the declared inventory (e.g., 10 laptops) doesn't match reality. Stock corruption on concurrent sales (confirmed) — two simultaneous sale requests can both read Stock=5, pass validation, and each decrement it, resulting in negative inventory. Missing sale lifecycle operations (confirmed) — sales cannot be edited or cancelled after creation, making error correction impossible without restarting the application.

**Verdict**: NEEDS_CHANGES

## High-level view

The three-module structure (Clientes, Productos, Ventas) uses separate controllers and full view sets for each, with navigation wired in _Layout.cshtml.

Sale creation uses a dynamic multi-product form with client-side JavaScript for add/remove rows and live total calculation, backed by server-side stock validation and automatic deduction.

Navigation properties (Cliente on Venta, Producto on VentaDetalle) are manually resolved in controller actions by matching IDs against DataStore collections, creating coupling between the data-access pattern and every consuming controller method.

The DataStore pre-populates two completed sales but never deducts their product quantities from stock. A laptop sale of 1 unit exists, yet the Laptop product still shows Stock=10, not 9.

The in-memory DataStore uses static mutable collections with no locking. Concurrent requests will race on stock reads, validation, and writes, allowing overselling and negative inventory.

Sale lifecycle ends at creation. There are no Edit or Delete actions for VentasController, so a mistaken sale entry cannot be corrected except by restarting the application.

<details>
<summary>Issues (3)</summary>

1. **Pre-seeded sale inventory mismatch** — DataStore initializes two historical sales (Venta Id=1: 1 Laptop + 2 Mice; Id=2: 1 Teclado + 1 Monitor) but never deducts these quantities from Productos stock. Actual available inventory is lower than displayed. Either remove sample Ventas, or initialize Productos stock accounting for them (Laptop: 9, Mouse: 48, Teclado: 29, Monitor: 14).

2. **Race condition on stock deduction** — DataStore collections are static and unsynchronized. Two simultaneous sale POST requests for the same product will both read the current stock, both pass validation, and both decrement, resulting in double-deduction or negative stock. Add locking around the validation-and-commit section in VentasController.Create, or document that the system is single-user only.

3. **No sale correction mechanism** — VentasController has no Edit or Delete actions. A sale with incorrect customer or quantities cannot be reversed or corrected without restarting the application. Either implement sale cancellation (restore stock, remove from DataStore) or add a warning in the Create view that sales are final.

</details>

<details>
<summary>Details</summary>

## Pre-seeded sale inventory inconsistency

DataStore.cs initializes `Ventas` with two completed sales from prior days:

- Venta Id=1: 1 Laptop (ProductoId=1) + 2 Mice (ProductoId=2)
- Venta Id=2: 1 Teclado (ProductoId=3) + 1 Monitor (ProductoId=4)

The `Productos` list declares initial stock values:

```
Laptop: Stock = 10
Mouse: Stock = 50
Teclado: Stock = 30
Monitor: Stock = 15
```

These two datasets are inconsistent. If the two sales were real, current stock should be Laptop=9, Mouse=48, Teclado=29, Monitor=14. Instead, the full initial stock is still shown, creating phantom inventory. Users see availability that doesn't account for the historical transactions displayed in Ventas Index.

The root cause: sample data construction skips the stock-deduction step that VentasController.Create performs. Fix by either removing the pre-seeded Ventas entirely, or adjusting the Productos initial stock values downward to reflect the two prior sales.

## Concurrent access and stock deduction race

The Create action in VentasController follows this sequence:

1. Validate at least one valid item
2. For each item, check `producto.Stock >= item.Cantidad`
3. If all checks pass, create Venta and VentaDetalle records
4. For each item, execute `producto.Stock -= item.Cantidad`
5. Add Venta to DataStore.Ventas

DataStore collections are static fields with no synchronization. Two requests executing concurrently will interleave:

```
Request A: reads Laptop.Stock = 5
Request B: reads Laptop.Stock = 5
Request A: validates quantity 3 <= 5, passes
Request B: validates quantity 4 <= 5, passes
Request A: deducts 3, Laptop.Stock = 2
Request B: deducts 4, Laptop.Stock = -2
```

Both sales succeed, resulting in negative stock. Subsequent sale attempts will fail validation (can't sell negative stock), but the corruption persists for the application lifetime. ASP.NET Core handles multiple simultaneous requests by default, so this race is realistic even with multiple browser tabs or slightly delayed network requests.

Mitigation requires synchronization around the validation-check-commit block. A `lock` statement on a shared object (e.g., a static `_ventasLock`) in VentasController.Create would serialize sale processing. Alternatively, document a single-user assumption and rely on deployment constraints.

## No Edit or Delete for sales

VentasController implements three actions: Index, Details, Create. The Ventas Index and Details views have no links for editing or deleting a sale.

Sales are immutable after creation. If a user selects the wrong customer, or enters incorrect quantities, or accidentally duplicates a sale, they have no UI path to correct it. The stock deduction is permanent (within the app session). The only recovery mechanism is restarting the application to reset DataStore to initial state, which loses all other work.

The original requirement ("registrar reservas y consultar las ventas") emphasizes registration and querying, not reversal, so the absence of Edit/Delete may be intentional. However, Clientes and Productos modules both include full CRUD, creating an asymmetry: mistakes in customer or product data are correctable; mistakes in sales are not.

If immutability is deliberate, the Create view should warn users that sales are final. If it's an oversight, adding a Delete action would require restoring stock for each VentaDetalle before removing the Venta from DataStore.

## Multi-product sale form and validation

The Ventas Create view uses a `<template>` element to define a product row structure. JavaScript clones the template on "Agregar Producto" button click, replacing the placeholder `INDEX` with an incrementing counter to generate unique form field names (`Items[0].ProductoId`, `Items[1].ProductoId`, etc.).

The product `<select>` options carry `data-price` and `data-stock` attributes. The `updateTotal()` function reads these attributes to calculate row subtotals and the overall sale total. This provides immediate feedback but is purely cosmetic; the server recalculates everything.

On form submission, the VentaViewModel.Items collection arrives as a list of `{ ProductoId, Cantidad }` pairs. The controller filters out rows where `ProductoId == 0` or `Cantidad <= 0`, then validates the remaining items.

Validation has three layers: at least one valid item exists (non-empty after filtering), each referenced ProductoId exists in DataStore.Productos, and each product has sufficient stock for the requested quantity. If any layer fails, the controller adds a ModelState error, repopulates ViewBag, and returns the form. If all pass, it creates VentaDetalle records using the product's current Precio (not the client-side data-price, which could be stale or tampered), sums subtotals, deducts stock, and commits the Venta.

The client-side stock display (e.g., "Monitor - $200.00 (Stock: 15)") is snapshot data from when the page loaded. If another user buys 10 monitors while the form is open, this user still sees Stock: 15 and might attempt to buy 10 themselves. Server validation catches this and returns an error message.

## Navigation property resolution pattern

Venta and VentaDetalle models include navigation properties (Cliente, Producto) that are nullable reference types. DataStore does not maintain these relationships; it only stores the ID fields (ClienteId, ProductoId).

Controller actions that present Venta data (VentasController.Index, Details) manually resolve these properties:

```csharp
venta.Cliente = DataStore.Clientes.FirstOrDefault(c => c.Id == venta.ClienteId);
foreach (var detalle in venta.Detalles)
{
    detalle.Producto = DataStore.Productos.FirstOrDefault(p => p.Id == detalle.ProductoId);
}
```

Every action that reads Ventas must remember to do this, or the views will display null for customer and product names. The Details view assumes these properties are populated; it renders `@Model.Cliente.Nombre` without null checks.

If a ClienteId or ProductoId references a deleted entity (not currently possible since there's no delete for Clientes/Productos with foreign key constraints, but the data model allows it), the navigation property remains null and the view throws a NullReferenceException at render time.

</details>

<details>
<summary>File map</summary>

**Models (6 files):**
- DataStore.cs — static collections for Clientes, Productos, Ventas; sample data; ID counters
- Cliente.cs — Id, Nombre, Email, Telefono; Required/EmailAddress validation attributes
- Producto.cs — Id, Nombre, Descripcion, Precio, Stock; Range validation for Price and Stock
- Venta.cs — Id, ClienteId, Fecha, Total; Detalles collection; Cliente navigation property
- VentaDetalle.cs — Id, VentaId, ProductoId, Cantidad, PrecioUnitario; computed Subtotal; Producto navigation property
- VentaViewModel.cs — ClienteId, Items list for form binding in Create action

**Controllers (3 files):**
- ClientesController.cs — full CRUD (Index, Create, Edit, Delete, Details)
- ProductosController.cs — full CRUD (Index, Create, Edit, Delete, Details)
- VentasController.cs — Index (list all sales with Cliente resolved), Create (multi-product form with stock validation and deduction), Details (sale with Detalles and navigation properties resolved)

**Views - Clientes (5 files):**
- Index, Create, Edit, Details, Delete — standard Bootstrap-styled CRUD views

**Views - Productos (5 files):**
- Index, Create, Edit, Details, Delete — standard Bootstrap-styled CRUD views

**Views - Ventas (3 files):**
- Index.cshtml — table of sales with customer name and total
- Create.cshtml — cliente dropdown, dynamic product rows using template and inline JavaScript (addProductRow, removeProductRow, updateTotal functions), live total display
- Details.cshtml — sale header (customer, date, total) and table of line items with product names

**Layout and navigation:**
- Views/Shared/_Layout.cshtml — navbar with GestorVentas brand, links to Home, Clientes, Productos, Ventas

**Note:** The build-result.md mentioned `wwwroot/js/ventas.js` as created, but this file does not exist in the filesystem. The JavaScript for sale creation is embedded in the `@section Scripts` block of Views/Ventas/Create.cshtml instead. This is functionally equivalent but contradicts the file inventory in the build report.

</details>
