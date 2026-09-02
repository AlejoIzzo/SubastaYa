using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.DTOs;
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
        public async Task<ActionResult<IEnumerable<SubastaDTO>>> GetAll()
        {
            var subastas = await _service.GetAllAsync();

            return Ok(subastas);
        }
    }
}
