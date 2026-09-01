using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Domain.Entities
{
    public class Puja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SubastaId { get; set; }

        [Required]
        [ForeignKey("Usuario_Id")]
        public int CompradorId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; }

        // Navigators
        public Subasta Subasta { get; set; } = null!;
        public Usuario Comprador { get; set; } = null!;
    }
}