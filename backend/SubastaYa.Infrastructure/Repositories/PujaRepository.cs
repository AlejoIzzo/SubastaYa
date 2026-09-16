using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.Interfaces;
using SubastaYa.Data;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Infrastructure.Repositories
{
    public class PujaRepository : IPujaRepository
    {
        private readonly SubastaYaContext _context;

        public PujaRepository(SubastaYaContext context)
        {
            _context = context;
        }

        public async Task<Subasta?> GetSubastaParaPujarAsync(int subastaId)
        {
            // Se incluye la lista de Pujas para poder determinar la puja líder actual
            return await _context.Subastas
                .Include(s => s.Pujas)
                .FirstOrDefaultAsync(s => s.Id == subastaId);
        }


        public async Task<IEnumerable<PujaDTO>> GetPujasBySubastaIdAsync(int subastaId)
        {
            return await _context.Pujas
                .Where(p => p.SubastaId == subastaId)
                .OrderByDescending(p => p.Fecha)
                .Select(p => new PujaDTO
                {
                    Id = p.Id,
                    SubastaId = p.SubastaId,
                    SubastaTitulo = p.Subasta.Titulo,
                    CompradorId = p.CompradorId,
                    CompradorNombre = p.Comprador.Nombre,
                    Monto = p.Monto,
                    Fecha = DateTime.SpecifyKind(p.Fecha, DateTimeKind.Utc)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PujaDTO>> GetAllPujasAsync(int? usuarioId = null)
        {
            var query = _context.Pujas.AsQueryable();

            if (usuarioId.HasValue)
            {
                query = query.Where(p => p.CompradorId == usuarioId.Value);
            }

            return await query
                .OrderByDescending(p => p.Fecha)
                .Select(p => new PujaDTO
                {
                    Id = p.Id,
                    SubastaId = p.SubastaId,
                    SubastaTitulo = p.Subasta.Titulo,
                    CompradorId = p.CompradorId,
                    CompradorNombre = p.Comprador.Nombre,
                    Monto = p.Monto,
                    Fecha = DateTime.SpecifyKind(p.Fecha, DateTimeKind.Utc)
                })
                .ToListAsync();
        }


        public async Task AgregarAsync(Puja puja)
        {
            await _context.Pujas.AddAsync(puja);
        }

        public async Task GuardarCambiosAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrenciaException("La subasta fue modificada por otra oferta en este mismo instante. Por favor, actualice la vista e intente nuevamente.");
            }
        }
        public async Task<decimal> GetTotalPujadoUsuario(int usuarioId)
        {
            return await _context.Pujas
                .Where(p => p.CompradorId == usuarioId)
                .SumAsync(p => (decimal?)p.Monto) ?? 0m;
        }
    }
}
