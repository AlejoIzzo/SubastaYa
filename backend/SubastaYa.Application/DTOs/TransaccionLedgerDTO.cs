using System;

namespace SubastaYa.Application.DTOs
{
    public class TransaccionLedgerDTO
    {
        public int Id { get; set; }
        public int BilleteraId { get; set; }
        public int? SubastaId { get; set; }
        public string? SubastaTitulo { get; set; }
        public string Tipo { get; set; } = string.Empty; // DEPOSITO, RETENCION, LIBERACION, PAGO, COBRO
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
