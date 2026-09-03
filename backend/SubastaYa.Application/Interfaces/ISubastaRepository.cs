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
        Task<SubastaDetalleDTO?> GetByIdAsync(int id, int ultimasPujasLimit = 5);
        Task<IEnumerable<Subasta>> GetAllAsync();
        Task<IEnumerable<SubastaCatalogDTO>> GetSubastaCatalog();
        Task CrearAsync(Subasta subasta);
        void Update(Subasta subasta);
        void Delete(Subasta subasta);
    }
}
