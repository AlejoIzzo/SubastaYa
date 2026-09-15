using SubastaYa.Application.DTOs;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubastaYa.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISubastaRepository _subastaRepository;
        private readonly IPujaRepository _pujaRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository, ISubastaRepository subastaRepository, IPujaRepository pujaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _subastaRepository = subastaRepository;
            _pujaRepository = pujaRepository;
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllUsuariosAsync()
        {
            return await _usuarioRepository.GetAllUsuariosDTOAsync();
        }

        public async Task<UsuarioDTO?> GetUsuarioByIdAsync(int id)
        {
            return await _usuarioRepository.GetUsuarioDTOByIdAsync(id);
        }

        public async Task<UsuarioDashboardDTO> GetUsuarioDashboard(int id)
        {
            decimal totalRecaudado = await _subastaRepository.GetTotalRecaudadoUsuario(id);
            decimal totalPujado = await _pujaRepository.GetTotalPujadoUsuario(id);
            int subastasActivas = await _subastaRepository.GetSubastasActivasUsuario(id);
            int subastasGanadas = await _subastaRepository.GetSubastasGanadasUsuario(id);

            var subastas = await _subastaRepository.GetSubastasDeUsuario(id);

            var participaciones = await _subastaRepository.GetSubastasParticipacionesDeUsuario(id);

            return new UsuarioDashboardDTO
            {
                UsuarioEstadisticas = new UsuarioEstadisticasDTO
                {
                    SubastasActivas = subastasActivas,
                    SubastasGanadas = subastasGanadas,
                    TotalRecaudado = totalRecaudado,
                    TotalPujado = totalPujado
                },
                UsuariosParticipacionSubastas = participaciones,
                UsuarioSubastas = subastas
            };
        }
    }
}
