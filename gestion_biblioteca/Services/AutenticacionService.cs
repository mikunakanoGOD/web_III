using gestion_biblioteca.Data;
using gestion_biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblioteca.Services
{
    public class AutenticacionService : IAutenticacionService
    {
        private readonly ApplicationDbContext _context;

        public AutenticacionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ValidarCredencialesAsync(string email, string password)
        {
            // Buscar usuario activo por email
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Activo);

            if (usuario == null)
                return null;

            // Verificar contraseña: soporta tanto texto plano (seed) como BCrypt
            bool passwordValido = VerificarPassword(password, usuario.PasswordHash);

            return passwordValido ? usuario : null;
        }

        public string HashearPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        public bool VerificarPassword(string password, string hash)
        {
            // Si el hash empieza con $2, es BCrypt; si no, comparamos directamente (seed inicial)
            if (hash.StartsWith("$2"))
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }

            // Compatibilidad con contraseñas de seed en texto plano
            return password == hash;
        }
    }
}
