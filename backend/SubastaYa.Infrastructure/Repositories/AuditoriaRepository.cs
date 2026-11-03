using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Interfaces;
using SubastaYa.Data;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Infrastructure.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly SubastaYaContext _context;

        public AuditoriaRepository(SubastaYaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditoriaLog>> GetAuditoriasAsync(string? entidad = null, string? accion = null, int? entidadId = null, int limit = 100)
        {
            var query = _context.AuditoriaLogs
                .Include(a => a.Usuario)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(entidad))
                query = query.Where(a => a.Entidad == entidad.Trim().ToUpper());

            if (!string.IsNullOrWhiteSpace(accion))
                query = query.Where(a => a.Accion == accion.Trim().ToUpper());

            if (entidadId.HasValue)
                query = query.Where(a => a.EntidadId == entidadId.Value);

            return await query
                .OrderByDescending(a => a.Fecha)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditoriaLog>> GetAuditoriasBySubastaAsync(int subastaId)
        {
            return await _context.AuditoriaLogs
                .Include(a => a.Usuario)
                .AsNoTracking()
                .Where(a => a.Entidad == "SUBASTA" && a.EntidadId == subastaId)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task AgregarAsync(AuditoriaLog log)
        {
            await _context.AuditoriaLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
