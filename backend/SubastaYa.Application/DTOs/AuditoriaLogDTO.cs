using System;

namespace SubastaYa.Application.DTOs
{
    public class AuditoriaLogDTO
    {
        public int Id { get; set; }
        public string Entidad { get; set; } = string.Empty;
        public int EntidadId { get; set; }
        public string Accion { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public DateTime Fecha { get; set; }
        public string? DetalleJson { get; set; }
    }
}
