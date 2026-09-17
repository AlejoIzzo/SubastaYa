using System;

namespace SubastaYa.Application.DTOs
{
    public class UsuarioDashboardDTO
    {
        public UsuarioEstadisticasDTO UsuarioEstadisticas { get; set; } = new UsuarioEstadisticasDTO();
        public PagedResultDTO<UsuarioSubastaDTO> UsuarioSubastas { get; set; } = new PagedResultDTO<UsuarioSubastaDTO>();
        public PagedResultDTO<UsuarioParticipacionSubastaDTO> UsuarioParticipacionSubastas { get; set; } = new PagedResultDTO<UsuarioParticipacionSubastaDTO>();
    }
}
