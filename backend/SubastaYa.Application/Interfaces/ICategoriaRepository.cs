using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<CategoriaDTO>> GetAllCategoriasDTO();
    }
}
