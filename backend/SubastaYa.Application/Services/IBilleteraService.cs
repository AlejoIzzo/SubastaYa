using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.Services
{
    public interface IBilleteraService
    {
        Task<BilleteraDTO?> GetByUsuarioIdAsync(int usuarioId);
        Task<BilleteraDTO?> GetByIdAsync(int id);
        Task<BilleteraDTO> CargarSaldoAsync(int usuarioId, decimal monto);
        Task<IEnumerable<TransaccionLedgerDTO>> GetTransaccionesAsync(int usuarioId);
    }
}
