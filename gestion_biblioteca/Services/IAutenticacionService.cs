using gestion_biblioteca.Models;

namespace gestion_biblioteca.Services
{
    public interface IAutenticacionService
    {
        /// <summary>
        /// Valida credenciales y retorna el usuario si son correctas, null si no.
        /// </summary>
        Task<Usuario?> ValidarCredencialesAsync(string email, string password);

        /// <summary>
        /// Hashea una contraseña en texto plano usando BCrypt.
        /// </summary>
        string HashearPassword(string password);

        /// <summary>
        /// Verifica si una contraseña en texto plano coincide con su hash.
        /// </summary>
        bool VerificarPassword(string password, string hash);
    }
}
