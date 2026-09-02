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

        public async Task<SubastaDetalleDTO?> GetByIdAsync(int id, int ultimasPujasLimit = 5)
        {
            return await _context.Subastas
                .Where(s => s.Id == id)
                .Select(s => new SubastaDetalleDTO
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    Descripcion = s.Descripcion,
                    PrecioBase = s.PrecioBase,
                    UrlImagen = s.UrlImagen,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    Estado = s.Estado,
                    IncrementoMinimo = s.IncrementoMinimo,

                    VendedorNombre = s.Vendedor.Nombre,

                    PujaActual = s.Pujas.Select(p => (decimal?)p.Monto).Max() ?? s.PrecioBase,
                    CantidadPujas = s.Pujas.Count(),
                    UltimasPujas = s.Pujas
                        .OrderByDescending(p => p.Fecha)
                        .Take(ultimasPujasLimit)
                        .Select(p => new PujaDTO
                        {
                            Id = p.Id,
                            Monto = p.Monto,
                            Fecha = p.Fecha,
                            CompradorNombre = p.Comprador.Nombre
                        }).ToList()
                }).FirstOrDefaultAsync();
            
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
