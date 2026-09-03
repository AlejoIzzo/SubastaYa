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
    public class BilleteraRepository : IBilleteraRepository
    {
        private readonly SubastaYaContext _context;

        public BilleteraRepository(SubastaYaContext context)
        {
            _context = context;
        }

        public async Task<Billetera?> GetByIdAsync(int id)
        {
            return await _context.Billeteras
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Billetera?> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Billeteras
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(b => b.UsuarioId == usuarioId);
        }

        public async Task<BilleteraDTO?> GetDtoByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Billeteras
                .Where(b => b.UsuarioId == usuarioId)
                .Select(b => new BilleteraDTO
                {
                    Id = b.Id,
                    UsuarioId = b.UsuarioId,
                    UsuarioNombre = b.Usuario.Nombre,
                    SaldoTotal = b.SaldoTotal,
                    SaldoRetenido = b.SaldoRetenido,
                    SaldoDisponible = b.SaldoDisponible,
                    Transacciones = b.TransaccionesLedger
                        .OrderByDescending(t => t.Fecha)
                        .Select(t => new TransaccionLedgerDTO
                        {
                            Id = t.Id,
                            BilleteraId = t.BilleteraId,
                            SubastaId = t.SubastaId,
                            SubastaTitulo = t.Subasta != null ? t.Subasta.Titulo : null,
                            Tipo = t.Tipo,
                            Monto = t.Monto,
                            Fecha = t.Fecha
                        }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<BilleteraDTO?> GetDtoByIdAsync(int id)
        {
            return await _context.Billeteras
                .Where(b => b.Id == id)
                .Select(b => new BilleteraDTO
                {
                    Id = b.Id,
                    UsuarioId = b.UsuarioId,
                    UsuarioNombre = b.Usuario.Nombre,
                    SaldoTotal = b.SaldoTotal,
                    SaldoRetenido = b.SaldoRetenido,
                    SaldoDisponible = b.SaldoDisponible,
                    Transacciones = b.TransaccionesLedger
                        .OrderByDescending(t => t.Fecha)
                        .Select(t => new TransaccionLedgerDTO
                        {
                            Id = t.Id,
                            BilleteraId = t.BilleteraId,
                            SubastaId = t.SubastaId,
                            SubastaTitulo = t.Subasta != null ? t.Subasta.Titulo : null,
                            Tipo = t.Tipo,
                            Monto = t.Monto,
                            Fecha = t.Fecha
                        }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TransaccionLedgerDTO>> GetTransaccionesByUsuarioIdAsync(int usuarioId)
        {
            return await _context.TransaccionLedger
                .Where(t => t.Billetera.UsuarioId == usuarioId)
                .OrderByDescending(t => t.Fecha)
                .Select(t => new TransaccionLedgerDTO
                {
                    Id = t.Id,
                    BilleteraId = t.BilleteraId,
                    SubastaId = t.SubastaId,
                    SubastaTitulo = t.Subasta != null ? t.Subasta.Titulo : null,
                    Tipo = t.Tipo,
                    Monto = t.Monto,
                    Fecha = t.Fecha
                }).ToListAsync();
        }

        public async Task AgregarTransaccionAsync(TransaccionLedger transaccion)
        {
            await _context.TransaccionLedger.AddAsync(transaccion);
        }

        public async Task AgregarAuditoriaAsync(AuditoriaLog log)
        {
            await _context.AuditoriaLogs.AddAsync(log);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
