using gestion_biblioteca.Models;

namespace gestion_biblioteca.ViewModels
{
    /// <summary>
    /// Datos del usuario almacenados en sesión
    /// </summary>
    public class UsuarioSesionViewModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Rol Rol { get; set; }

        public bool EsAdministrador => Rol == Rol.Administrador;
        public bool EsBibliotecario => Rol == Rol.Bibliotecario;
        public bool EsUsuario => Rol == Rol.Usuario;

        public string RolNombre => Rol switch
        {
            Rol.Administrador => "Administrador",
            Rol.Bibliotecario => "Bibliotecario",
            Rol.Usuario       => "Usuario",
            _                 => "Desconocido"
        };
    }
}
