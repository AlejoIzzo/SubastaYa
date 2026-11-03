using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Services;

namespace SubastaYa.API.Controllers
{
    [ApiController]
    [Route("api/auditorias")]
    public class AuditoriasController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriasController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
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
            var logs = await _auditoriaService.GetAuditoriasAsync(entidad, accion, entidadId);
            return Ok(logs);
        }

        /// <summary>
        /// Obtiene todos los eventos de auditoría relacionados a una subasta en particular (anti-sniping, cierres, rechazos).
        /// </summary>
        [HttpGet("subastas/{subastaId}")]
        public async Task<ActionResult<IEnumerable<AuditoriaLogDTO>>> GetAuditoriasBySubasta(int subastaId)
        {
            var logs = await _auditoriaService.GetAuditoriasBySubastaAsync(subastaId);
            return Ok(logs);
        }
    }
}
