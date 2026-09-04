using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.Services
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDTO>> GetAllCategorias();
    }
}
