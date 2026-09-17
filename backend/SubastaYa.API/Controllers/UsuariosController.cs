using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Services;

namespace SubastaYa.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetAllUsuarios()
        {
            var usuarios = await _usuarioService.GetAllUsuariosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuarioById(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
                return NotFound(new { message = $"Usuario con ID {id} no encontrado" });

            return Ok(usuario);
        }

        /// <summary>
        /// Obtiene el historial de todas las ofertas realizadas por el usuario (Módulo 5: Mis Compras / Pujas).
        /// </summary>
        [HttpGet("{id}/pujas")]
        public async Task<ActionResult<IEnumerable<PujaDTO>>> GetPujasByUsuario(int id, [FromServices] IPujaService pujaService)
        {
            var pujas = await pujaService.GetAllPujasAsync(id);
            return Ok(pujas);
        }

        [HttpGet("{id}/dashboard")]
        public async Task<ActionResult<UsuarioDashboardDTO>> GetUsuarioDashboard(int id)
        {
            var dashboard = await _usuarioService.GetUsuarioDashboard(id);
            return Ok(dashboard);
        }

        [HttpGet("{id}/subastas")]
        public async Task<ActionResult<PagedResultDTO<UsuarioSubastaDTO>>> GetSubastasByUsuario(
            int id, 
            [FromQuery] int pagina = 1, 
            [FromQuery] int tamanioPagina = 5)
        {
            var result = await _usuarioService.GetSubastasDeUsuarioPaginadasAsync(id, pagina, tamanioPagina);
            return Ok(result);
        }

        [HttpGet("{id}/participaciones")]
        public async Task<ActionResult<PagedResultDTO<UsuarioParticipacionSubastaDTO>>> GetParticipacionesByUsuario(
            int id, 
            [FromQuery] int pagina = 1, 
            [FromQuery] int tamanioPagina = 5)
        {
            var result = await _usuarioService.GetSubastasParticipacionesDeUsuarioPaginadasAsync(id, pagina, tamanioPagina);
            return Ok(result);
        }
    }
}
