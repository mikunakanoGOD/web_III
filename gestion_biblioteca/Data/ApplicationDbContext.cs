using gestion_biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblioteca.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- DbSets (tablas) ---
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =============================================
            // Configuración de la entidad Usuario
            // =============================================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Rol).IsRequired();

                // El email debe ser único
                entity.HasIndex(u => u.Email).IsUnique();

                // Ignorar propiedad calculada (no va a la BD)
                entity.Ignore(u => u.NombreCompleto);
            });

            // =============================================
            // Configuración de la entidad Libro
            // =============================================
            modelBuilder.Entity<Libro>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(l => l.Autor).IsRequired().HasMaxLength(150);
                entity.Property(l => l.ISBN).IsRequired().HasMaxLength(20);
                entity.Property(l => l.Editorial).IsRequired().HasMaxLength(150);
                entity.Property(l => l.Categoria).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Descripcion).HasMaxLength(500);

                // ISBN único
                entity.HasIndex(l => l.ISBN).IsUnique();
            });

            // =============================================
            // Configuración de la entidad Prestamo
            // =============================================
            modelBuilder.Entity<Prestamo>(entity =>
            {
                entity.HasKey(p => p.Id);

                // Relación Prestamo → Usuario (solicitante)
                entity.HasOne(p => p.Usuario)
                      .WithMany(u => u.Prestamos)
                      .HasForeignKey(p => p.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación Prestamo → Libro
                entity.HasOne(p => p.Libro)
                      .WithMany(l => l.Prestamos)
                      .HasForeignKey(p => p.LibroId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación Prestamo → Bibliotecario (opcional)
                entity.HasOne(p => p.Bibliotecario)
                      .WithMany()
                      .HasForeignKey(p => p.BibliotecarioId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });

            // =============================================
            // Seed Data: usuarios iniciales
            // =============================================
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Admin",
                    Apellido = "Sistema",
                    Email = "admin@biblioteca.com",
                    // Contraseña: Admin123 (hash BCrypt simulado con SHA256 para seed)
                    PasswordHash = "Admin123",
                    Rol = Rol.Administrador,
                    Activo = true,
                    FechaRegistro = new DateTime(2024, 1, 1)
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "biblio@biblioteca.com",
                    PasswordHash = "Biblio123",
                    Rol = Rol.Bibliotecario,
                    Activo = true,
                    FechaRegistro = new DateTime(2024, 1, 1)
                },
                new Usuario
                {
                    Id = 3,
                    Nombre = "María",
                    Apellido = "García",
                    Email = "usuario@biblioteca.com",
                    PasswordHash = "User123",
                    Rol = Rol.Usuario,
                    Activo = true,
                    FechaRegistro = new DateTime(2024, 1, 1)
                }
            );

            // =============================================
            // Seed Data: libros de ejemplo
            // =============================================
            modelBuilder.Entity<Libro>().HasData(
                new Libro
                {
                    Id = 1,
                    Titulo = "Cien años de soledad",
                    Autor = "Gabriel García Márquez",
                    ISBN = "978-0-06-088328-7",
                    Editorial = "Editorial Sudamericana",
                    AñoPublicacion = 1967,
                    Categoria = "Literatura",
                    Descripcion = "Novela del realismo mágico latinoamericano.",
                    CantidadTotal = 5,
                    CantidadDisponible = 5,
                    Estado = EstadoLibro.Disponible,
                    FechaIngreso = new DateTime(2024, 1, 1)
                },
                new Libro
                {
                    Id = 2,
                    Titulo = "Clean Code",
                    Autor = "Robert C. Martin",
                    ISBN = "978-0-13-235088-4",
                    Editorial = "Prentice Hall",
                    AñoPublicacion = 2008,
                    Categoria = "Tecnología",
                    Descripcion = "Guía para escribir código limpio y mantenible.",
                    CantidadTotal = 3,
                    CantidadDisponible = 3,
                    Estado = EstadoLibro.Disponible,
                    FechaIngreso = new DateTime(2024, 1, 1)
                },
                new Libro
                {
                    Id = 3,
                    Titulo = "El Principito",
                    Autor = "Antoine de Saint-Exupéry",
                    ISBN = "978-84-204-4648-2",
                    Editorial = "Salamandra",
                    AñoPublicacion = 1943,
                    Categoria = "Literatura",
                    Descripcion = "Cuento filosófico y poético para todas las edades.",
                    CantidadTotal = 4,
                    CantidadDisponible = 4,
                    Estado = EstadoLibro.Disponible,
                    FechaIngreso = new DateTime(2024, 1, 1)
                }
            );
        }
    }
}
