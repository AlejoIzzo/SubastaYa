using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Application.DTOs
{
    public class CargarSaldoDTO
    {
        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(1, 100000000, ErrorMessage = "El monto a depositar debe ser mayor a 0")]
        public decimal Monto { get; set; }
    }
}
