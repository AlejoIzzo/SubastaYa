using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubastaYa.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<UsuarioDTO>> GetAllUsuariosDTOAsync();
        Task<UsuarioDTO?> GetUsuarioDTOByIdAsync(int id);

        Task<Usuario?> GetByIdAsync(int id);
    }
}
