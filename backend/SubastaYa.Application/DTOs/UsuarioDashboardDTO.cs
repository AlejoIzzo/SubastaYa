using System;

namespace SubastaYa.Application.DTOs
{
    public class UsuarioDashboardDTO
    {
        public UsuarioEstadisticasDTO UsuarioEstadisticas { get; set; } = new UsuarioEstadisticasDTO();
        public IEnumerable<UsuarioSubastaDTO> UsuarioSubastas { get; set; } = new List<UsuarioSubastaDTO>();
        public IEnumerable<UsuarioParticipacionSubastaDTO> UsuarioParticipacionSubastas { get; set; } = new List<UsuarioParticipacionSubastaDTO>();
    }
}
