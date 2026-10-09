using gestion_biblioteca.Models;
using gestion_biblioteca.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gestion_biblioteca.Filters
{
    /// <summary>
    /// Filtro que verifica si el usuario está autenticado y,
    /// opcionalmente, si tiene alguno de los roles permitidos.
    /// Uso: [RequiereAutenticacion] o [RequiereAutenticacion(Rol.Administrador, Rol.Bibliotecario)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequiereAutenticacionAttribute : ActionFilterAttribute
    {
        private readonly Rol[] _rolesPermitidos;

        /// <summary>Sin restricción de rol: solo requiere estar autenticado.</summary>
        public RequiereAutenticacionAttribute()
        {
            _rolesPermitidos = Array.Empty<Rol>();
        }

        /// <summary>Requiere autenticación y uno de los roles indicados.</summary>
        public RequiereAutenticacionAttribute(params Rol[] roles)
        {
            _rolesPermitidos = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var usuario = SesionService.ObtenerUsuario(session);

            // No autenticado → redirigir al login
            if (usuario == null)
            {
                var returnUrl = context.HttpContext.Request.Path;
                context.Result = new RedirectToActionResult("Login", "Account",
                    new { returnUrl });
                return;
            }

            // Verificar rol si se especificaron roles permitidos
            if (_rolesPermitidos.Length > 0 && !_rolesPermitidos.Contains(usuario.Rol))
            {
                context.Result = new RedirectToActionResult("AccesoDenegado", "Account", null);
                return;
            }

            // Pasar el usuario a ViewData para que esté disponible en todas las vistas
            if (context.Controller is Controller controller)
            {
                controller.ViewData["UsuarioSesion"] = usuario;
            }

            base.OnActionExecuting(context);
        }
    }
}
