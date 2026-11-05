using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Application.DTOs
{
    public class CrearPujaDTO
    {
        [Required(ErrorMessage = "El ID del comprador es obligatorio.")]
        public int CompradorId { get; set; }

        [Required(ErrorMessage = "El monto de la puja es obligatorio.")]
        public decimal Monto { get; set; }
    }
}
