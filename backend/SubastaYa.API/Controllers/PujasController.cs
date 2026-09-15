using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SubastaYa.API.Hubs;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubastaYa.API.Controllers
{
    [ApiController]
    [Route("api/subastas/{subastaId}/pujas")]
    public class PujasController : ControllerBase
    {
        private readonly IPujaService _pujaService;
        private readonly IHubContext<SubastaHub> _hubContext;
        public PujasController(IPujaService pujaService, IHubContext<SubastaHub> hubContext)
        {
            _pujaService = pujaService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Obtiene el historial completo de ofertas de una subasta específica.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PujaDTO>>> GetPujas(int subastaId)
        {
            var pujas = await _pujaService.GetPujasBySubastaIdAsync(subastaId);
            return Ok(pujas);
        }

        /// <summary>
        /// Obtiene todas las ofertas de la plataforma, con filtro opcional por ID de usuario (?usuarioId=X).
        /// </summary>
        [HttpGet("/api/pujas")]
        public async Task<ActionResult<IEnumerable<PujaDTO>>> GetAllPujas([FromQuery] int? usuarioId)
        {
            var pujas = await _pujaService.GetAllPujasAsync(usuarioId);
            return Ok(pujas);
        }

        /// <summary>
        /// Registra una nueva puja en la subasta.
        /// Maneja bloqueo atómico de saldos (Escrow), extensión anti-sniping y concurrencia optimista (HTTP 409).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PujaResultadoDTO>> CrearPuja(int subastaId, [FromBody] CrearPujaDTO dto)
        {
            try
            {
                var resultado = await _pujaService.CrearPujaAsync(subastaId, dto);

                // Le avisamos en vivo a todos los clientes de esa subasta
                await _hubContext.Clients.Group($"Subasta-{subastaId}").SendAsync("RecibirPuja", resultado);
                return StatusCode(StatusCodes.Status201Created, resultado);
            }
            catch (ConcurrenciaException ex)
            {
                // Manejo explícito de concurrencia optimista (Requerimiento evaluado del TP)
                return StatusCode(StatusCodes.Status409Conflict, new
                {
                    status = 409,
                    error = "Conflict",
                    message = ex.Message
                });
            }
            catch (DominioException ex)
            {
                // Errores de validación de negocio (monto insuficiente, fondos insuficientes, etc.)
                return BadRequest(new
                {
                    status = 400,
                    error = "Bad Request",
                    message = ex.Message
                });
            }
        }
    }
}
