using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.Services
{
    public interface IPujaService
    {
        /// Registra una nueva oferta validando reglas de negocio, ejecutando el traspaso atómico de fondos (Escrow)
        /// y aplicando la regla anti-sniping si corresponde.
        Task<PujaResultadoDTO> CrearPujaAsync(int subastaId, CrearPujaDTO dto);

        /// Obtiene todas las pujas de una subasta específica.
        Task<IEnumerable<PujaDTO>> GetPujasBySubastaIdAsync(int subastaId);

        /// Obtiene todas las pujas de la plataforma (con filtro opcional por usuario).
        Task<IEnumerable<PujaDTO>> GetAllPujasAsync(int? usuarioId = null);

        /// Registra en la tabla de auditoría inmutable los intentos de puja rechazados por concurrencia o reglas de negocio críticas.
        Task RegistrarAuditoriaRechazoAsync(int subastaId, int? usuarioId, decimal monto, string accion, string motivo);
    }
}
