namespace gestion_biblioteca.Models
{
    /// <summary>
    /// Roles disponibles en el sistema de biblioteca
    /// </summary>
    public enum Rol
    {
        Administrador = 1,
        Bibliotecario = 2,
        Usuario = 3
    }

    /// <summary>
    /// Estado de un préstamo
    /// </summary>
    public enum EstadoPrestamo
    {
        Pendiente = 1,
        Aprobado = 2,
        Rechazado = 3,
        Devuelto = 4
    }

    /// <summary>
    /// Estado de disponibilidad de un libro
    /// </summary>
    public enum EstadoLibro
    {
        Disponible = 1,
        Prestado = 2,
        NoDisponible = 3
    }
}
