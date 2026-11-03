using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.Services
{
    public interface IAuditoriaService
    {
        Task<IEnumerable<AuditoriaLogDTO>> GetAuditoriasAsync(string? entidad = null, string? accion = null, int? entidadId = null);
        Task<IEnumerable<AuditoriaLogDTO>> GetAuditoriasBySubastaAsync(int subastaId);
    }
}
