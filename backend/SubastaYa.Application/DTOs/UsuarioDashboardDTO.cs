using System;

namespace SubastaYa.Application.DTOs
{
    public class UsuarioDashboardDTO
    {
        public UsuarioEstadisticasDTO UsuarioEstadisticas { get; set; }
        public IEnumerable<UsuarioSubastaDTO> UsuarioSubastas { get; set; } = new List<UsuarioSubastaDTO>();
        public IEnumerable<UsuarioParticipacionSubastaDTO> UsuariosParticipacionSubastas { get; set; } = new List<UsuarioParticipacionSubastaDTO>();
    }
}
