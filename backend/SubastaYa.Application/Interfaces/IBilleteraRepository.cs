using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Interfaces
{
    public interface IBilleteraRepository
    {
        Task<Billetera?> GetByIdAsync(int id);
        Task<Billetera?> GetByUsuarioIdAsync(int usuarioId);
        Task<BilleteraDTO?> GetDtoByUsuarioIdAsync(int usuarioId);
        Task<BilleteraDTO?> GetDtoByIdAsync(int id);
        Task<IEnumerable<TransaccionLedgerDTO>> GetTransaccionesByUsuarioIdAsync(int usuarioId);
        Task AgregarTransaccionAsync(TransaccionLedger transaccion);
        Task AgregarAuditoriaAsync(AuditoriaLog log);
        Task GuardarCambiosAsync();
    }
}
