using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Interfaces
{
    public interface IAuditoriaRepository
    {
        Task<IEnumerable<AuditoriaLog>> GetAuditoriasAsync(string? entidad = null, string? accion = null, int? entidadId = null, int limit = 100);
        Task<IEnumerable<AuditoriaLog>> GetAuditoriasBySubastaAsync(int subastaId);
        Task AgregarAsync(AuditoriaLog log);
    }
}
