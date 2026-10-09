namespace gestion_biblioteca.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // Mensaje personalizado de error para mostrar al usuario
        public string? Mensaje { get; set; }

        // Código HTTP del error (404, 403, 500, etc.)
        public int? StatusCode { get; set; }

        // Título del error para mostrar en la vista
        public string Titulo => StatusCode switch
        {
            403 => "Acceso Denegado",
            404 => "Página No Encontrada",
            500 => "Error Interno del Servidor",
            _   => "Ha Ocurrido un Error"
        };
    }
}
