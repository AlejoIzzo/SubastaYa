using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.DTOs
{
    public class CrearSubastaDTO
    {
        [Required]
        public string Titulo { get; set; } = string.Empty;
        [Required]
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        public int CategoriaId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PrecioBase { get; set; }
        [Required]
        public string UrlImagen { get; set; } = string.Empty;
        public DateTime? FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        [Required]
        public int VendedorId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal IncrementoMinimo { get; set; }
    }
}
