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

        public async Task CrearAsync(Subasta subasta)
        {
            await _context.AddAsync(subasta);
            await _context.SaveChangesAsync();
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

                    CategoriaNombre = s.Categoria.Nombre,
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
        public async Task<IEnumerable<SubastaCatalogDTO>> GetSubastaCatalog(SubastaFiltroDTO? filtro = null)
        {
            var query = _context.Subastas.AsQueryable();

            if (filtro != null)
            {
                // Filtro por Estado (ACTIVA, PROGRAMADA, FINALIZADA, DESIERTA)
                if (!string.IsNullOrWhiteSpace(filtro.Estado))
                {
                    var estadoUpper = filtro.Estado.Trim().ToUpper();
                    query = query.Where(s => s.Estado == estadoUpper);
                }

                // Filtro por Categoría
                if (filtro.CategoriaId.HasValue && filtro.CategoriaId.Value > 0)
                {
                    query = query.Where(s => s.CategoriaId == filtro.CategoriaId.Value);
                }

                // Búsqueda por texto en título o descripción
                if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
                {
                    var busqueda = filtro.Busqueda.Trim();
                    query = query.Where(s => s.Titulo.Contains(busqueda) || s.Descripcion.Contains(busqueda));
                }

                // Filtro por Rango de Precios (comparando contra la puja actual o precio base)
                if (filtro.PrecioMin.HasValue)
                {
                    query = query.Where(s => (s.Pujas.Select(p => (decimal?)p.Monto).Max() ?? s.PrecioBase) >= filtro.PrecioMin.Value);
                }

                if (filtro.PrecioMax.HasValue)
                {
                    query = query.Where(s => (s.Pujas.Select(p => (decimal?)p.Monto).Max() ?? s.PrecioBase) <= filtro.PrecioMax.Value);
                }

                // Ordenamiento
                query = filtro.Orden?.ToLower() switch
                {
                    "tiempo_asc" or "menor_tiempo" => query.OrderBy(s => s.FechaFin),
                    "tiempo_desc" => query.OrderByDescending(s => s.FechaFin),
                    "puja_desc" or "mayor_puja" => query.OrderByDescending(s => s.Pujas.Select(p => (decimal?)p.Monto).Max() ?? s.PrecioBase),
                    "puja_asc" or "menor_puja" => query.OrderBy(s => s.Pujas.Select(p => (decimal?)p.Monto).Max() ?? s.PrecioBase),
                    "recientes" => query.OrderByDescending(s => s.FechaInicio),
                    _ => query.OrderBy(s => s.FechaFin)
                };
            }
            else
            {
                query = query.OrderBy(s => s.FechaFin);
            }

            return await query
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
                    CategoriaId = s.CategoriaId,
                    CategoriaNombre = s.Categoria.Nombre,
                    CantidadPujas = s.Pujas.Count(),
                    PujaActual = s.Pujas.Select(p => (decimal?)p.Monto).Max() ?? s.PrecioBase,
                    VendedorNombre = s.Vendedor.Nombre
                }).ToListAsync();
        }

        public void Update(Subasta subasta)
        {
            throw new NotImplementedException();
        }
    }
}
