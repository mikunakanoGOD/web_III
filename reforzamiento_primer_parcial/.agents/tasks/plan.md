# Implementation Plan: Sistema de Gestión de Clientes, Productos y Ventas

This plan implements a modular sales management system with in-memory storage, no scaffolding, and no Entity Framework. The project is a standard ASP.NET Core 8.0 MVC application using Bootstrap 5.

## Project Context

- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# with nullable reference types enabled
- **Build Command**: `dotnet build`
- **Run Command**: `dotnet run`
- **Verification**: Build success + manual testing through browser
- **Namespace**: reforzamiento_primer_parcial
- **Architecture**: MVC with in-memory storage (static DataStore class)
- **No Entity Framework**: All data stored in static lists

## Implementation Steps

- [ ] 1. Create the DataStore class with in-memory collections for all entities.
      Static class with static lists for Clientes, Productos, Ventas, and VentaDetalles. Include thread-safe ID generation and seed data initialization.
      Files: Models/DataStore.cs (new file)
      Verify: `dotnet build` succeeds without errors.

- [ ] 2. Create the Cliente model.
      Properties: Id (int), Nombre (string), Email (string), Telefono (string). All strings are non-nullable with validation attributes.
      Files: Models/Cliente.cs (new file)
      Verify: `dotnet build` succeeds without errors.

- [ ] 3. Create the Producto model.
      Properties: Id (int), Nombre (string), Descripcion (string), Precio (decimal), Stock (int). Include validation attributes for required fields and ranges.
      Files: Models/Producto.cs (new file)
      Verify: `dotnet build` succeeds without errors.

- [ ] 4. Create the Venta and VentaDetalle models.
      Venta properties: Id (int), ClienteId (int), Fecha (DateTime), Total (decimal), Cliente (navigation property), Detalles (List<VentaDetalle>).
      VentaDetalle properties: Id (int), VentaId (int), ProductoId (int), Cantidad (int), PrecioUnitario (decimal), Subtotal (decimal), Producto (navigation property).
      Files: Models/Venta.cs (new file), Models/VentaDetalle.cs (new file)
      Verify: `dotnet build` succeeds without errors.

- [ ] 5. Create the VentaViewModel for the Create form.
      Properties: ClienteId (int), Items (List<VentaItemViewModel>). VentaItemViewModel has ProductoId (int) and Cantidad (int).
      Files: Models/ViewModels/VentaViewModel.cs (new file), Models/ViewModels/VentaItemViewModel.cs (new file)
      Verify: `dotnet build` succeeds without errors.

- [ ] 6. Initialize DataStore with seed data in Program.cs.
      Add sample Clientes, Productos, and one Venta with detalles immediately after `var app = builder.Build();` to provide test data.
      Files: Program.cs (modify)
      Verify: `dotnet build` succeeds without errors.

- [ ] 7. Create ClientesController with full CRUD actions.
      Actions: Index (list all), Create (GET/POST), Edit (GET/POST with id), Details (GET with id), Delete (GET/POST with id). All actions work with DataStore.Clientes.
      Files: Controllers/ClientesController.cs (new file)
      Verify: `dotnet build` succeeds without errors.

- [ ] 8. Create Clientes views: Index, Create, Edit, Details, Delete.
      Index: table with action links. Create/Edit: form with Nombre, Email, Telefono fields. Details: display all properties. Delete: confirmation page. All use Bootstrap 5 styling following the pattern in existing _Layout.cshtml.
      Files: Views/Clientes/Index.cshtml (new), Views/Clientes/Create.cshtml (new), Views/Clientes/Edit.cshtml (new), Views/Clientes/Details.cshtml (new), Views/Clientes/Delete.cshtml (new)
      Verify: `dotnet build` succeeds. Run `dotnet run`, navigate to /Clientes/Index, verify all CRUD operations work.

- [ ] 9. Create ProductosController with full CRUD actions.
      Actions: Index (list all), Create (GET/POST), Edit (GET/POST with id), Details (GET with id), Delete (GET/POST with id). All actions work with DataStore.Productos. Include stock validation.
      Files: Controllers/ProductosController.cs (new file)
      Verify: `dotnet build` succeeds without errors.

- [ ] 10. Create Productos views: Index, Create, Edit, Details, Delete.
       Index: table with Nombre, Precio, Stock, and action links. Create/Edit: form with Nombre, Descripcion, Precio (decimal input), Stock (number input). Details: display all properties with currency formatting. Delete: confirmation page. All use Bootstrap 5 styling.
       Files: Views/Productos/Index.cshtml (new), Views/Productos/Create.cshtml (new), Views/Productos/Edit.cshtml (new), Views/Productos/Details.cshtml (new), Views/Productos/Delete.cshtml (new)
       Verify: `dotnet build` succeeds. Run `dotnet run`, navigate to /Productos/Index, verify all CRUD operations work.

- [ ] 11. Create VentasController with List, Create (GET/POST), and Details actions.
       Index: list all ventas with Cliente name, Fecha, Total. Create GET: form with ClienteId dropdown and dynamic multi-product line items (JavaScript). Create POST: validate stock, calculate totals, create Venta and VentaDetalles, decrement stock. Details: show Venta header, Cliente info, and all line items with product names.
       Files: Controllers/VentasController.cs (new file)
       Verify: `dotnet build` succeeds without errors.

- [ ] 12. Create Ventas views: Index, Create, Details.
       Index: table with columns Cliente, Fecha, Total, Details link. Create: form with ClienteId select, dynamic table rows for line items (ProductoId select + Cantidad input), Add/Remove row buttons, Calculate Total button. Details: header with Cliente and Fecha, table with all line items showing Producto, Cantidad, PrecioUnitario, Subtotal, and Total. All use Bootstrap 5 styling.
       Files: Views/Ventas/Index.cshtml (new), Views/Ventas/Create.cshtml (new), Views/Ventas/Details.cshtml (new)
       Verify: `dotnet build` succeeds without errors.

- [ ] 13. Add JavaScript for dynamic line items in Ventas/Create view.
       Script to add/remove product line item rows dynamically. Each row has ProductoId dropdown and Cantidad input. Include client-side total calculation. Place in @section Scripts block.
       Files: Views/Ventas/Create.cshtml (modify)
       Verify: `dotnet build` succeeds. Run `dotnet run`, navigate to /Ventas/Create, verify adding/removing rows works, verify submission creates venta with multiple products.

- [ ] 14. Update _Layout.cshtml navigation to include links to Clientes, Productos, and Ventas modules.
       Replace Privacy link with links to /Clientes/Index, /Productos/Index, /Ventas/Index. Keep Home link.
       Files: Views/Shared/_Layout.cshtml (modify)
       Verify: `dotnet build` succeeds. Run `dotnet run`, verify navigation menu shows all three module links and they work correctly.

- [ ] 15. Update Home/Index.cshtml to show a welcome dashboard with links to each module.
       Replace default welcome text with a simple dashboard: title "Sistema de Gestión de Ventas" and three Bootstrap cards linking to Clientes, Productos, and Ventas with brief descriptions.
       Files: Views/Home/Index.cshtml (modify)
       Verify: `dotnet build` succeeds. Run `dotnet run`, navigate to /, verify dashboard shows three module cards with working links.

- [ ] 16. Final integration test of the complete system.
       Run `dotnet run` and test the complete workflow: create a client, create products, create a sale with multiple products, verify stock decrements, view sale details, verify all navigation links work.
       Files: (no file changes, testing only)
       Verify: Complete user workflow succeeds: create Cliente → create 2+ Productos → create Venta with multiple line items → verify sale appears in Index with correct total → view Details shows all products → verify stock was decremented in Productos/Index.
