using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Domain.Entities
{
    public class Billetera
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Usuario_Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo_Total { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo_Retenido { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo_Disponible { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}