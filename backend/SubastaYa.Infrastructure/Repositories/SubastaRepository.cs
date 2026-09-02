using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Interfaces;
using SubastaYa.Data;
using SubastaYa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Infrastructure.Repositories
{
    public class SubastaRepository : ISubastaRepository
    {
        private readonly SubastaYaContext _context;

        public SubastaRepository(SubastaYaContext context)
        {
            _context = context;
        }

        public Task AddAsync(Subasta subasta)
        {
            throw new NotImplementedException();
        }

        public void Delete(Subasta subasta)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Subasta>> GetAllAsync()
        {
            return await _context.Subastas.ToListAsync(); 
        }

        public Task<Subasta?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        // devolver DTO directamente desde el repositorio para que SQL haga la agregación
        public async Task<IEnumerable<SubastaCatalogDTO>> GetSubastaCatalog()
        {
            return await _context.Subastas
                .Select(s => new SubastaCatalogDTO
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    Descripcion = s.Descripcion,
                    PrecioBase = s.PrecioBase,
                    UrlImagen = s.UrlImagen,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    Estado = s.Estado,

                    CantidadPujas = s.Pujas.Count(),
                    PujaActual = s.Pujas.Select(p => (decimal?)p.Monto).Max() ?? s.PrecioBase, // castear a nullable para caso donde no haya pujas

                    VendedorNombre = s.Vendedor.Nombre
                }).ToListAsync();
        }

        public void Update(Subasta subasta)
        {
            throw new NotImplementedException();
        }
    }
}
