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

        public async Task<SubastaDetalleDTO?> GetByIdAsync(int id, int ultimasPujasLimit = 15)
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
                    FechaInicio = DateTime.SpecifyKind(s.FechaInicio, DateTimeKind.Utc),
                    FechaFin = DateTime.SpecifyKind(s.FechaFin, DateTimeKind.Utc),
                    Estado = s.Estado,
                    IncrementoMinimo = s.IncrementoMinimo,

                    CategoriaNombre = s.Categoria.Nombre,
                    VendedorNombre = s.Vendedor.Nombre,
                    VendedorId = s.VendedorId,

                    PujaActual = s.Pujas
                        .OrderByDescending(p => p.Monto)
                        .Select(p => new PujaDTO
                        {
                            Id = p.Id,
                            SubastaId = p.SubastaId,
                            SubastaTitulo = s.Titulo,
                            CompradorId = p.CompradorId,
                            CompradorNombre = p.Comprador.Nombre,
                            Monto = p.Monto,
                            // construir fecha especificando utc para que no se pierda la zona horaria en la serialización a JSON
                            Fecha = DateTime.SpecifyKind(p.Fecha, DateTimeKind.Utc)
                        }).FirstOrDefault(),

                    CantidadPujas = s.Pujas.Count(),
                    UltimasPujas = s.Pujas
                        .OrderByDescending(p => p.Fecha)
                        .Take(ultimasPujasLimit)
                        .Select(p => new PujaDTO
                        {
                            Id = p.Id,
                            SubastaId = p.SubastaId,
                            SubastaTitulo = s.Titulo,
                            CompradorId = p.CompradorId,
                            CompradorNombre = p.Comprador.Nombre,
                            Monto = p.Monto,
                            Fecha = DateTime.SpecifyKind(p.Fecha, DateTimeKind.Utc)
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

                // Filtro por Vendedor (Mis Publicaciones)
                if (filtro.VendedorId.HasValue && filtro.VendedorId.Value > 0)
                {
                    query = query.Where(s => s.VendedorId == filtro.VendedorId.Value);
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

            // Paginación opcional
            if (filtro != null && filtro.Pagina.HasValue && filtro.Pagina.Value > 0 && filtro.TamanioPagina.HasValue && filtro.TamanioPagina.Value > 0)
            {
                int skip = (filtro.Pagina.Value - 1) * filtro.TamanioPagina.Value;
                query = query.Skip(skip).Take(filtro.TamanioPagina.Value);
            }

            return await query
                .Select(s => new SubastaCatalogDTO
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    Descripcion = s.Descripcion,
                    PrecioBase = s.PrecioBase,
                    UrlImagen = s.UrlImagen,
                    FechaInicio = DateTime.SpecifyKind(s.FechaInicio, DateTimeKind.Utc),
                    FechaFin = DateTime.SpecifyKind(s.FechaFin, DateTimeKind.Utc),
                    Estado = s.Estado,
                    CategoriaId = s.CategoriaId,
                    CategoriaNombre = s.Categoria.Nombre,
                    CantidadPujas = s.Pujas.Count(),
                    PujaActual = s.Pujas.Select(p => (decimal?)p.Monto).Max() ?? s.PrecioBase,
                    VendedorNombre = s.Vendedor.Nombre
                }).ToListAsync();
        }

        public async Task<int> GetSubastasActivasUsuario(int usuarioId)
        {
            return await _context.Subastas
                .CountAsync(s => s.VendedorId == usuarioId && s.Estado == "ACTIVA");
        }

        public async Task<IEnumerable<UsuarioSubastaDTO>> GetSubastasDeUsuario(int usuarioId)
        {
            return await _context.Subastas
                .Where(s => s.VendedorId == usuarioId)
                .OrderByDescending(s => s.FechaInicio)
                .Select(s => new UsuarioSubastaDTO
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    UrlImagen = s.UrlImagen,
                    PrecioBase = s.PrecioBase,
                    CantidadPujas = s.Pujas.Count(),
                    FechaInicio = DateTime.SpecifyKind(s.FechaInicio, DateTimeKind.Utc),
                    FechaFin = DateTime.SpecifyKind(s.FechaFin, DateTimeKind.Utc),
                    Estado = s.Estado,
                    VendedorId = s.VendedorId,
                    PujaLider = s.Pujas
                        .OrderByDescending(p => p.Monto)
                        .Select(p => new PujaDTO
                        {
                            Id = p.Id,
                            SubastaId = p.SubastaId,
                            SubastaTitulo = s.Titulo,
                            CompradorId = p.CompradorId,
                            CompradorNombre = p.Comprador.Nombre,
                            Monto = p.Monto,
                            Fecha = DateTime.SpecifyKind(p.Fecha, DateTimeKind.Utc)
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<int> GetSubastasGanadasUsuario(int usuarioId)
        {
            var ganadas = await _context.Subastas
                .Where(s => s.Estado == "FINALIZADA" && s.Pujas.Any())
                .Select(s => s.Pujas.OrderByDescending(p => p.Monto).Select(p => p.CompradorId).FirstOrDefault())
                .ToListAsync();

            return ganadas.Count(compradorId => compradorId == usuarioId);
        }

        public async Task<IEnumerable<UsuarioParticipacionSubastaDTO>> GetSubastasParticipacionesDeUsuario(int usuarioId)
        {
            return await _context.Subastas
                .Where(s => s.Pujas.Any(p => p.CompradorId == usuarioId))
                .OrderByDescending(s => s.Pujas.Where(p => p.CompradorId == usuarioId).Max(p => p.Fecha))
                .Select(s => new UsuarioParticipacionSubastaDTO
                {
                    Id = s.Id,
                    Titulo = s.Titulo,
                    UrlImagen = s.UrlImagen,
                    FechaInicio = DateTime.SpecifyKind(s.FechaInicio, DateTimeKind.Utc),
                    FechaFin = DateTime.SpecifyKind(s.FechaFin, DateTimeKind.Utc),
                    Estado = s.Estado,
                    VendedorId = s.VendedorId,
                    PujaLider = s.Pujas
                        .OrderByDescending(p => p.Monto)
                        .Select(p => new PujaDTO
                        {
                            Id = p.Id,
                            SubastaId = p.SubastaId,
                            SubastaTitulo = s.Titulo,
                            CompradorId = p.CompradorId,
                            CompradorNombre = p.Comprador.Nombre,
                            Monto = p.Monto,
                            Fecha = DateTime.SpecifyKind(p.Fecha, DateTimeKind.Utc)
                        })
                        .FirstOrDefault(),
                    UltimaPujaUsuario = s.Pujas
                        .Where(p => p.CompradorId == usuarioId)
                        .OrderByDescending(p => p.Fecha)
                        .Select(p => new PujaDTO
                        {
                            Id = p.Id,
                            SubastaId = p.SubastaId,
                            SubastaTitulo = s.Titulo,
                            CompradorId = p.CompradorId,
                            CompradorNombre = p.Comprador.Nombre,
                            Monto = p.Monto,
                            Fecha = DateTime.SpecifyKind(p.Fecha, DateTimeKind.Utc)
                        })
                        .First()
                })
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRecaudadoUsuario(int usuarioId)
        {
            return await _context.TransaccionLedger
                .Where(t => t.Billetera.UsuarioId == usuarioId && t.Tipo == "COBRO")
                .SumAsync(t => (decimal?)t.Monto) ?? 0m;
        }

        public void Update(Subasta subasta)
        {
            _context.Subastas.Update(subasta);
        }
    }
}
