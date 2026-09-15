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

        public async Task<Billetera?> GetBilleteraByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Billeteras
                .FirstOrDefaultAsync(b => b.UsuarioId == usuarioId);
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int usuarioId)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId);
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
                    Fecha = p.Fecha
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
                    Fecha = p.Fecha
                })
                .ToListAsync();
        }

        public async Task GuardarPujaConTransaccionAsync(
            Subasta subasta,
            Puja nuevaPuja,
            Billetera billeteraNuevoPostor,
            TransaccionLedger transaccionRetencion,
            Billetera? billeteraPostorAnterior,
            TransaccionLedger? transaccionLiberacion,
            AuditoriaLog? logAntiSniping)
        {
            // Iniciamos la transacción atómica
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //Si había un postor anterior con fondos retenidos, se registra la liberación y se actualiza su saldo
                if (billeteraPostorAnterior != null && transaccionLiberacion != null)
                {
                    await _context.TransaccionLedger.AddAsync(transaccionLiberacion);
                    _context.Billeteras.Update(billeteraPostorAnterior);
                }

                //Se retienen los fondos del nuevo postor y se crea su comprobante en el Ledger
                await _context.TransaccionLedger.AddAsync(transaccionRetencion);
                _context.Billeteras.Update(billeteraNuevoPostor);

                //Se inserta la nueva puja
                await _context.Pujas.AddAsync(nuevaPuja);

                //Se actualiza la subasta (si se extendió el tiempo por anti-sniping o para refrescar Version)
                _context.Subastas.Update(subasta);

                //Si se disparó la regla anti-sniping, se guarda el log inmutable de auditoría
                if (logAntiSniping != null)
                {
                    await _context.AuditoriaLogs.AddAsync(logAntiSniping);
                }

                //Guardar cambios en la base de datos (aquí SQL Server evalúa el Optimistic Locking)
                await _context.SaveChangesAsync();

                //Si todo fue exitoso, confirmamos la transacción
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si dos usuarios ofertaron al mismo milisegundo, la versión cambió.
                // Se revierte la transacción para no dejar datos corruptos y se lanza la excepción de concurrencia.
                await transaction.RollbackAsync();
                throw new ConcurrenciaException("La subasta fue modificada por otra oferta en este mismo instante. Por favor, actualice la vista e intente nuevamente.");
            }
            catch
            {
                // Cualquier otro fallo produce un rollback completo
                await transaction.RollbackAsync();
                throw;
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
