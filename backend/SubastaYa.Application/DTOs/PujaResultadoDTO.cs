using System;

namespace SubastaYa.Application.DTOs
{
    public class PujaResultadoDTO
    {
        public int Id { get; set; }
        public int SubastaId { get; set; }
        public int CompradorId { get; set; }
        public string CompradorNombre { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public bool AntiSnipingActivado { get; set; }
        public DateTime FechaFinSubasta { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
