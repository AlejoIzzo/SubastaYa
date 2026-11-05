using System.ComponentModel.DataAnnotations;

namespace SubastaYa.Application.DTOs
{
    public class CargarSaldoDTO
    {
        [Required(ErrorMessage = "El monto es obligatorio")]
        public decimal Monto { get; set; }
    }
}
