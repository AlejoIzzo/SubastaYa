using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.DTOs
{
    public class SubastaDetalleDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioBase { get; set; }
        public decimal PujaActual { get; set; }
        public int CantidadPujas { get; set; }
        public string UrlImagen { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string VendedorNombre { get; set; } = string.Empty;
        public ICollection<PujaDTO> UltimasPujas { get; set; } = new List<PujaDTO>();
        public decimal IncrementoMinimo { get; set; }
    }
}
