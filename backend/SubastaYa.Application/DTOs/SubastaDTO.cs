using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.DTOs
{
    public class SubastaDTO
    {
        public int Id { get; set; }
        
        public string Titulo { get; set; } = string.Empty;
        
        public string Descripcion { get; set; } = string.Empty;

        public decimal PrecioBase { get; set; }

        public string UrlImagen { get; set; } = string.Empty;
        
        public DateTime FechaInicio { get; set; }
        
        public DateTime FechaFin { get; set; }

        public string Estado { get; set; }
    }
}
