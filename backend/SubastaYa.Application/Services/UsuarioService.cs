using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Interfaces;

namespace SubastaYa.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllUsuariosAsync()
        {
            return await _usuarioRepository.GetAllUsuariosDTOAsync();
        }

        public async Task<UsuarioDTO?> GetUsuarioByIdAsync(int id)
        {
            return await _usuarioRepository.GetUsuarioDTOByIdAsync(id);
        }
    }
}
