using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.DTOs;
using SubastaYa.Data;

namespace SubastaYa.API.Controllers
{
    [ApiController]
    [Route("api/auditorias")]
    public class AuditoriasController : ControllerBase
    {
        private readonly SubastaYaContext _context;

        public AuditoriasController(SubastaYaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene el registro inmutable de auditoría (Audit Log) con filtros opcionales.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditoriaLogDTO>>> GetAuditorias(
            [FromQuery] string? entidad = null,
            [FromQuery] string? accion = null,
            [FromQuery] int? entidadId = null)
        {
            var query = _context.AuditoriaLogs
                .Include(a => a.Usuario)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(entidad))
                query = query.Where(a => a.Entidad == entidad.Trim().ToUpper());

            if (!string.IsNullOrWhiteSpace(accion))
                query = query.Where(a => a.Accion == accion.Trim().ToUpper());

            if (entidadId.HasValue)
                query = query.Where(a => a.EntidadId == entidadId.Value);

            var logs = await query
                .OrderByDescending(a => a.Fecha)
                .Take(100)
                .Select(a => new AuditoriaLogDTO
                {
                    Id = a.Id,
                    Entidad = a.Entidad,
                    EntidadId = a.EntidadId,
                    Accion = a.Accion,
                    UsuarioId = a.UsuarioId,
                    UsuarioNombre = a.Usuario != null ? a.Usuario.Nombre : null,
                    Fecha = System.DateTime.SpecifyKind(a.Fecha, System.DateTimeKind.Utc),
                    DetalleJson = a.DetalleJson
                })
                .ToListAsync();

            return Ok(logs);
        }

        /// <summary>
        /// Obtiene todos los eventos de auditoría relacionados a una subasta en particular (anti-sniping, cierres, rechazos).
        /// </summary>
        [HttpGet("subastas/{subastaId}")]
        public async Task<ActionResult<IEnumerable<AuditoriaLogDTO>>> GetAuditoriasBySubasta(int subastaId)
        {
            var logs = await _context.AuditoriaLogs
                .Include(a => a.Usuario)
                .Where(a => a.Entidad == "SUBASTA" && a.EntidadId == subastaId)
                .OrderByDescending(a => a.Fecha)
                .Select(a => new AuditoriaLogDTO
                {
                    Id = a.Id,
                    Entidad = a.Entidad,
                    EntidadId = a.EntidadId,
                    Accion = a.Accion,
                    UsuarioId = a.UsuarioId,
                    UsuarioNombre = a.Usuario != null ? a.Usuario.Nombre : null,
                    Fecha = System.DateTime.SpecifyKind(a.Fecha, System.DateTimeKind.Utc),
                    DetalleJson = a.DetalleJson
                })
                .ToListAsync();

            return Ok(logs);
        }
    }
}
