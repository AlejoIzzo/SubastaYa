using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.API.Controllers
{
    [ApiController]
    [Route("api/subastas")]
    public class SubastasController : ControllerBase
    {
        private readonly ISubastaService _service;

        public SubastasController(ISubastaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubastaDTO>>> GetSubastaCatalog()
        {
            var subastas = await _service.GetSubastaCatalog();

            return Ok(subastas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<SubastaDetalleDTO>>> GetSubastaById(int id)
        {
            var subasta = await _service.GetByIdAsync(id, ultimasPujasLimit: 5);

            if (subasta == null)
                return NotFound();

            return Ok(subasta);
        }

        [HttpPost]
        public async Task<ActionResult<SubastaCreadaDTO>> CrearSubasta(CrearSubastaDTO dto)
        {
            try
            {
                var subastaCreadaDTO = await _service.CrearAsync(dto);

                return CreatedAtAction(
                    actionName: "CrearSubasta",
                    routeValues: new { id = subastaCreadaDTO.Id },
                    value: subastaCreadaDTO
                ); 
            } catch (DominioException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
