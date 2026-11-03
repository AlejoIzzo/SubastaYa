using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Interfaces;

namespace SubastaYa.Application.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IAuditoriaRepository _auditoriaRepository;

        public AuditoriaService(IAuditoriaRepository auditoriaRepository)
        {
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task<IEnumerable<AuditoriaLogDTO>> GetAuditoriasAsync(string? entidad = null, string? accion = null, int? entidadId = null)
        {
            var logs = await _auditoriaRepository.GetAuditoriasAsync(entidad, accion, entidadId);

            return logs.Select(a => new AuditoriaLogDTO
            {
                Id = a.Id,
                Entidad = a.Entidad,
                EntidadId = a.EntidadId,
                Accion = a.Accion,
                UsuarioId = a.UsuarioId,
                UsuarioNombre = a.Usuario?.Nombre,
                Fecha = DateTime.SpecifyKind(a.Fecha, DateTimeKind.Utc),
                DetalleJson = a.DetalleJson
            });
        }

        public async Task<IEnumerable<AuditoriaLogDTO>> GetAuditoriasBySubastaAsync(int subastaId)
        {
            var logs = await _auditoriaRepository.GetAuditoriasBySubastaAsync(subastaId);

            return logs.Select(a => new AuditoriaLogDTO
            {
                Id = a.Id,
                Entidad = a.Entidad,
                EntidadId = a.EntidadId,
                Accion = a.Accion,
                UsuarioId = a.UsuarioId,
                UsuarioNombre = a.Usuario?.Nombre,
                Fecha = DateTime.SpecifyKind(a.Fecha, DateTimeKind.Utc),
                DetalleJson = a.DetalleJson
            });
        }
    }
}
