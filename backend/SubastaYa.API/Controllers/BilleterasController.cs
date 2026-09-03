using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.Services;

namespace SubastaYa.API.Controllers
{
    [ApiController]
    [Route("api/billeteras")]
    public class BilleterasController : ControllerBase
    {
        private readonly IBilleteraService _billeteraService;

        public BilleterasController(IBilleteraService billeteraService)
        {
            _billeteraService = billeteraService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BilleteraDTO>> GetById(int id)
        {
            var billetera = await _billeteraService.GetByIdAsync(id);
            if (billetera == null)
                return NotFound(new { message = $"Billetera con ID {id} no encontrada" });

            return Ok(billetera);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<BilleteraDTO>> GetByUsuarioId(int usuarioId)
        {
            var billetera = await _billeteraService.GetByUsuarioIdAsync(usuarioId);
            if (billetera == null)
                return NotFound(new { message = $"No se encontró billetera para el usuario con ID {usuarioId}" });

            return Ok(billetera);
        }

        [HttpGet("usuario/{usuarioId}/transacciones")]
        public async Task<ActionResult<IEnumerable<TransaccionLedgerDTO>>> GetTransacciones(int usuarioId)
        {
            var transacciones = await _billeteraService.GetTransaccionesAsync(usuarioId);
            return Ok(transacciones);
        }

        [HttpPost("usuario/{usuarioId}/depositos")]
        public async Task<ActionResult<BilleteraDTO>> Depositar(int usuarioId, [FromBody] CargarSaldoDTO dto)
        {
            try
            {
                var billeteraActualizada = await _billeteraService.CargarSaldoAsync(usuarioId, dto.Monto);
                return Ok(billeteraActualizada);
            }
            catch (DominioException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
