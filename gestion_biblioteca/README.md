# 📚 Sistema de Gestión de Biblioteca - Universidad Privada del Valle

Sistema de gestión de biblioteca desarrollado en **ASP.NET Core MVC** para la administración de libros, usuarios y préstamos.

## 🎯 Características

### Roles del Sistema
- **Administrador**: Gestión completa de usuarios del sistema
- **Bibliotecario**: Gestión de catálogo de libros y préstamos
- **Usuario**: Consulta de catálogo y solicitud de préstamos

## 🛠️ Tecnologías

- ASP.NET Core 8.0 (MVC)
- Entity Framework Core
- SQL Server LocalDB
- Bootstrap 5
- jQuery

## 📋 Requisitos

- .NET 8.0 SDK
- Visual Studio 2022 o VS Code
- SQL Server LocalDB

## 🚀 Instalación

1. Clonar el repositorio
```bash
git clone [URL_DEL_REPOSITORIO]
cd gestion_biblioteca
```

2. Restaurar dependencias
```bash
dotnet restore
```

3. Aplicar migraciones
```bash
dotnet ef database update
```

4. Ejecutar la aplicación
```bash
dotnet run
```

## 📁 Estructura del Proyecto

```
gestion_biblioteca/
├── Controllers/      # Controladores MVC
├── Models/          # Modelos de datos
├── Views/           # Vistas Razor
├── Data/            # Contexto de base de datos
├── ViewModels/      # ViewModels y DTOs
└── wwwroot/         # Archivos estáticos
```

## 👥 Usuarios por Defecto

Después de ejecutar las migraciones, el sistema incluye usuarios de prueba:

- **Administrador**: admin@biblioteca.com / Admin123
- **Bibliotecario**: biblio@biblioteca.com / Biblio123
- **Usuario**: usuario@biblioteca.com / User123

## 📝 Funcionalidades por Rol

### Administrador
- Dashboard principal
- CRUD de usuarios (Administradores, Bibliotecarios, Usuarios)
- Gestión de roles

### Bibliotecario
- Dashboard con estadísticas
- CRUD de libros (catálogo)
- Gestión de préstamos (aprobar/rechazar)
- Ver historial de préstamos

### Usuario Final
- Ver catálogo de libros disponibles
- Solicitar préstamos
- Ver mis préstamos activos
- Ver historial de préstamos

## 🔒 Seguridad

- Sistema de autenticación con sesiones
- Autorización basada en roles
- Validación de datos en servidor y cliente

## 📦 Próximas Mejoras

- [ ] Implementar ASP.NET Identity
- [ ] Sistema de notificaciones por email
- [ ] Reportes en PDF
- [ ] Búsqueda avanzada de libros
- [ ] Sistema de multas por retraso

## 👨‍💻 Autor

Desarrollado como proyecto académico para Web III - Universidad Privada del Valle

## 📄 Licencia

Este proyecto es de uso académico.
