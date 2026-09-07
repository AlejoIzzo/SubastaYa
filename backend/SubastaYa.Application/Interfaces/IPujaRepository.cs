using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Interfaces
{
    public interface IPujaRepository
    {
        /// Obtiene la subasta con sus datos necesarios para evaluar la puja incluyendo sus pujas anteriores.
        Task<Subasta?> GetSubastaParaPujarAsync(int subastaId);

        /// Obtiene la billetera de un usuario.
        Task<Billetera?> GetBilleteraByUsuarioIdAsync(int usuarioId);

        /// Obtiene los datos del usuario comprador.
        Task<Usuario?> GetUsuarioByIdAsync(int usuarioId);

        /// Obtiene el historial de todas las ofertas realizadas en una subasta, ordenadas de más reciente a más antigua.
        Task<IEnumerable<PujaDTO>> GetPujasBySubastaIdAsync(int subastaId);

        /// Obtiene todas las pujas registradas en la plataforma, con filtro opcional por usuario.
        Task<IEnumerable<PujaDTO>> GetAllPujasAsync(int? usuarioId = null);

        /// Registra la nueva puja, los movimientos contables y la auditoría dentro de una transacción atómica.
        Task GuardarPujaConTransaccionAsync(
            Subasta subasta,
            Puja nuevaPuja,
            Billetera billeteraNuevoPostor,
            TransaccionLedger transaccionRetencion,
            Billetera? billeteraPostorAnterior,
            TransaccionLedger? transaccionLiberacion,
            AuditoriaLog? logAntiSniping
        );
    }
}
