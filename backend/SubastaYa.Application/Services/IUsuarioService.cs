using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> GetAllUsuariosAsync();
        Task<UsuarioDTO?> GetUsuarioByIdAsync(int id);
    }
}
