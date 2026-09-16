using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Interfaces;
using SubastaYa.Data;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SubastaYaContext _context;

        public UsuarioRepository(SubastaYaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllUsuariosDTOAsync()
        {
            return await _context.Usuarios
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    FechaRegistro = u.FechaRegistro,
                    BilleteraId = u.Billetera != null ? u.Billetera.Id : (int?)null
                })
                .ToListAsync();
        }

        public async Task<UsuarioDTO?> GetUsuarioDTOByIdAsync(int id)
        {
            return await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    FechaRegistro = u.FechaRegistro,
                    BilleteraId = u.Billetera != null ? u.Billetera.Id : (int?)null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
