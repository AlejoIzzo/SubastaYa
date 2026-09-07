using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<UsuarioDTO>> GetAllUsuariosDTOAsync();
        Task<UsuarioDTO?> GetUsuarioDTOByIdAsync(int id);
    }
}
