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
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly SubastaYaContext _context;

        public CategoriaRepository(SubastaYaContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CategoriaDTO>> GetAllCategoriasDTO()
        {
            return await _context.Categorias
                .Select(c => new CategoriaDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    UrlIcono = c.UrlIcono
                }).ToListAsync();
        }

    }
}
