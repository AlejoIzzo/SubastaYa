using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.Interfaces
{
    public interface ISubastaRepository
    {
        Task<SubastaDetalleDTO?> GetByIdAsync(int id, int ultimasPujasLimit = 15);
        Task<IEnumerable<Subasta>> GetAllAsync();
        Task<PagedResultDTO<SubastaCatalogDTO>> GetSubastaCatalog(SubastaFiltroDTO? filtro = null);
        Task CrearAsync(Subasta subasta);
        void Update(Subasta subasta);
        void Delete(Subasta subasta);
        Task<IEnumerable<UsuarioSubastaDTO>> GetSubastasDeUsuario(int usuarioId);
        Task<IEnumerable<UsuarioParticipacionSubastaDTO>> GetSubastasParticipacionesDeUsuario(int usuarioId);
        Task<decimal> GetTotalRecaudadoUsuario(int usuarioId);
        Task<int> GetSubastasGanadasUsuario(int usuarioId);
        Task<int> GetSubastasActivasUsuario(int usuarioId);
    }
}
