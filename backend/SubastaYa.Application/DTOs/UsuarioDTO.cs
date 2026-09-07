using System;

namespace SubastaYa.Application.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public int? BilleteraId { get; set; }
    }
}
