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
        [ForeignKey("Usuario_Id")]
        public int UsuarioId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoRetenido { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoDisponible { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }

        // navigators 
        public Usuario Usuario { get; set; } = null!;
        
        public ICollection<TransaccionLedger> TransaccionesLedger { get; set; } = new List<TransaccionLedger>();
    }
}