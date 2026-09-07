using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Application.DTOs
{
    public class CrearPujaDTO
    {
        [Required(ErrorMessage = "El ID del comprador es obligatorio.")]
        public int CompradorId { get; set; }

        [Required(ErrorMessage = "El monto de la puja es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Monto { get; set; }
    }
}
