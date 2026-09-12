using SubastaYa.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubastaYa.Application.Interfaces
{
    public interface ISubastaClosingRepository
    {
        Task<List<Subasta>> GetSubastasVencidasAsync();
        Task<List<Subasta>> GetSubastasProgramadasParaIniciarAsync();
        Task FinalizarSubastaConGanadorAsync(int subastaId, int ganadorId, int vendedorId, decimal monto);
        Task FinalizarSubastaDesiertaAsync(int subastaId);
        Task ActivarSubastaProgramadaAsync(int subastaId);
    }
}
