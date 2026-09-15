using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.DTOs
{
    public class UsuarioParticipacionSubastaDTO
    {
        public int Id { get; set; }
        public string UrlImagen { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public PujaDTO? PujaLider { get; set; }
        public PujaDTO UltimaPujaUsuario { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int VendedorId { get; set; }
    }
}
