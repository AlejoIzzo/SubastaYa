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


        /// Obtiene el historial de todas las ofertas realizadas en una subasta, ordenadas de más reciente a más antigua.
        Task<IEnumerable<PujaDTO>> GetPujasBySubastaIdAsync(int subastaId);

        /// Obtiene todas las pujas registradas en la plataforma, con filtro opcional por usuario.
        Task<IEnumerable<PujaDTO>> GetAllPujasAsync(int? usuarioId = null);

        Task AgregarAsync(Puja puja);

        Task GuardarCambiosAsync();

        Task<decimal> GetTotalPujadoUsuario(int usuarioId);
        Task RegistrarAuditoriaAsync(AuditoriaLog log);
    }
}
