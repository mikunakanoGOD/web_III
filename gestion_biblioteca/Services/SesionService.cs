using gestion_biblioteca.Models;
using gestion_biblioteca.ViewModels;
using System.Text.Json;

namespace gestion_biblioteca.Services
{
    /// <summary>
    /// Gestiona los datos del usuario autenticado en la sesión HTTP.
    /// </summary>
    public static class SesionService
    {
        private const string SesionKey = "UsuarioSesion";

        public static void GuardarUsuario(ISession session, Usuario usuario)
        {
            var datos = new UsuarioSesionViewModel
            {
                Id            = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email         = usuario.Email,
                Rol           = usuario.Rol
            };
            session.SetString(SesionKey, JsonSerializer.Serialize(datos));
        }

        public static UsuarioSesionViewModel? ObtenerUsuario(ISession session)
        {
            var json = session.GetString(SesionKey);
            return json is null ? null : JsonSerializer.Deserialize<UsuarioSesionViewModel>(json);
        }

        public static void CerrarSesion(ISession session)
        {
            session.Remove(SesionKey);
        }

        public static bool EstaAutenticado(ISession session)
        {
            return session.GetString(SesionKey) is not null;
        }
    }
}
