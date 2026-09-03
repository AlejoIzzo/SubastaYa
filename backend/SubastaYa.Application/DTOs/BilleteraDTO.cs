using System.Collections.Generic;

namespace SubastaYa.Application.DTOs
{
    public class BilleteraDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public decimal SaldoTotal { get; set; }
        public decimal SaldoRetenido { get; set; }
        public decimal SaldoDisponible { get; set; }
        public ICollection<TransaccionLedgerDTO> Transacciones { get; set; } = new List<TransaccionLedgerDTO>();
    }
}
