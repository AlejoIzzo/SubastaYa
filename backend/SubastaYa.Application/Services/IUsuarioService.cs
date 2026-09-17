using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> GetAllUsuariosAsync();
        Task<UsuarioDTO?> GetUsuarioByIdAsync(int id);
        Task<UsuarioDashboardDTO> GetUsuarioDashboard(int id, int paginaSubastas = 1, int tamanioSubastas = 5, int paginaParticipaciones = 1, int tamanioParticipaciones = 5);
        Task<PagedResultDTO<UsuarioSubastaDTO>> GetSubastasDeUsuarioPaginadasAsync(int usuarioId, int pagina = 1, int tamanioPagina = 5);
        Task<PagedResultDTO<UsuarioParticipacionSubastaDTO>> GetSubastasParticipacionesDeUsuarioPaginadasAsync(int usuarioId, int pagina = 1, int tamanioPagina = 5);
    }
}
