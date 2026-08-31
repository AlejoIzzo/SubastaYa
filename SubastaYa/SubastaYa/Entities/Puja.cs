using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Entities
{
    public class Puja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Subasta_Id { get; set; }

        [Required]
        public int Comprador_Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public DateTime Fecha_Puja { get; set; }

        // Propiedad de navegación
        [ForeignKey("Subasta_Id")]
        public Subasta Subasta { get; set; }
    }
}