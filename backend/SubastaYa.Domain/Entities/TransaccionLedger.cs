using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Domain.Entities
{
    public class TransaccionLedger
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Billetera_Id")]
        public int BilleteraId { get; set; }

        [ForeignKey("Subasta_Id")]
        public int? SubastaId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Tipo { get; set; } = string.Empty; // DEPOSITO, RETENCION, LIBERACION, PAGO, COBRO

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }
        
        [Required]
        public DateTime Fecha { get; set; }
        // navigators
        public Billetera Billetera { get; set; } = null!;
        public Subasta? Subasta { get; set; }
    }
}