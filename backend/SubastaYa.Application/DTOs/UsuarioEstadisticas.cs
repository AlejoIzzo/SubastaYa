using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.DTOs
{
    public class UsuarioEstadisticasDTO
    {
        public int SubastasActivas { get; set; }
        public int SubastasGanadas { get; set; }
        public decimal TotalPujado { get; set; }
        public decimal TotalRecaudado { get; set; }
    }
}
