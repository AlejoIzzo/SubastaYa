using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Interfaces;
using SubastaYa.Data;
using SubastaYa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SubastaYa.Infrastructure.Repositories
{
    public class SubastaClosingRepository : ISubastaClosingRepository
    {
        private readonly SubastaYaContext _context;

        public SubastaClosingRepository(SubastaYaContext context)
        {
            _context = context;
        }

        public async Task<List<Subasta>> GetSubastasVencidasAsync()
        {
            DateTime now = DateTime.UtcNow;
            return await _context.Subastas
                .Include(s => s.Pujas)
                .Where(s => s.Estado == "ACTIVA" && s.FechaFin <= now)
                .ToListAsync();
        }

        public async Task<List<Subasta>> GetSubastasProgramadasParaIniciarAsync()
        {
            DateTime now = DateTime.UtcNow;
            return await _context.Subastas
                .Where(s => s.Estado == "PROGRAMADA" && s.FechaInicio <= now)
                .ToListAsync();
        }

        public async Task FinalizarSubastaConGanadorAsync(int subastaId, int ganadorId, int vendedorId, decimal monto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                DateTime now = DateTime.UtcNow;

                var subasta = await _context.Subastas.FirstOrDefaultAsync(s => s.Id == subastaId);
                if (subasta == null) return;

                var billeteraGanador = await _context.Billeteras.FirstOrDefaultAsync(b => b.UsuarioId == ganadorId);
                var billeteraVendedor = await _context.Billeteras.FirstOrDefaultAsync(b => b.UsuarioId == vendedorId);

                if (billeteraGanador == null)
                    throw new InvalidOperationException($"No se encontró la billetera para el comprador ganador con UsuarioId {ganadorId}.");

                if (billeteraVendedor == null)
                    throw new InvalidOperationException($"No se encontró la billetera para el vendedor con UsuarioId {vendedorId}.");

                // 1. Cambiar estado de la subasta a FINALIZADA
                subasta.Estado = "FINALIZADA";
                _context.Subastas.Update(subasta);

                // 2. Liquidación atómica:
                // Obtener el saldo neto que realmente fue retenido para ESTA subasta en el libro mayor (Ledger)
                var retencionesSubasta = await _context.TransaccionLedger
                    .Where(t => t.BilleteraId == billeteraGanador.Id && t.SubastaId == subasta.Id && t.Tipo == "RETENCION")
                    .SumAsync(t => t.Monto);

                var liberacionesSubasta = await _context.TransaccionLedger
                    .Where(t => t.BilleteraId == billeteraGanador.Id && t.SubastaId == subasta.Id && t.Tipo == "LIBERACION")
                    .SumAsync(t => t.Monto);

                var montoRetenidoSubasta = retencionesSubasta - liberacionesSubasta;

                // Validación de solvencia: El saldo retenido en la billetera no puede ser menor a lo retenido por esta subasta
                if (billeteraGanador.SaldoRetenido < montoRetenidoSubasta)
                {
                    throw new InvalidOperationException(
                        $"Error de liquidación: la billetera {billeteraGanador.Id} posee un saldo retenido " +
                        $"(${billeteraGanador.SaldoRetenido:N2}) inferior al monto retenido por la subasta {subasta.Id} (${montoRetenidoSubasta:N2})."
                    );
                }

                // El comprador debe tener suficiente saldo total para el débito de adjudicación
                if (billeteraGanador.SaldoTotal < monto)
                {
                    throw new InvalidOperationException(
                        $"Error de liquidación: la billetera {billeteraGanador.Id} posee un saldo total " +
                        $"(${billeteraGanador.SaldoTotal:N2}) inferior al monto a liquidar (${monto:N2})."
                    );
                }

                // Descontar de SaldoRetenido únicamente lo que realmente estaba retenido por ESTA subasta
                billeteraGanador.SaldoRetenido -= montoRetenidoSubasta;

                // Descontar del SaldoTotal el monto liquidado definitivo
                billeteraGanador.SaldoTotal -= monto;

                // El saldo disponible se recalcula garantizando la coherencia financiera
                billeteraGanador.SaldoDisponible = billeteraGanador.SaldoTotal - billeteraGanador.SaldoRetenido;

                // Validación estricta de no-negatividad post-liquidación
                if (billeteraGanador.SaldoRetenido < 0 || billeteraGanador.SaldoDisponible < 0 || billeteraGanador.SaldoTotal < 0)
                {
                    throw new InvalidOperationException(
                        $"Inconsistencia financiera: saldo negativo detectado en la billetera {billeteraGanador.Id} tras liquidar la subasta {subasta.Id}."
                    );
                }

                _context.Billeteras.Update(billeteraGanador);

                // Registro en libro mayor (Ledger) para el comprador: PAGO
                var pagoLedger = new TransaccionLedger
                {
                    BilleteraId = billeteraGanador.Id,
                    SubastaId = subasta.Id,
                    Tipo = "PAGO",
                    Monto = monto,
                    Fecha = now
                };
                await _context.TransaccionLedger.AddAsync(pagoLedger);

                // - Al vendedor se le acredita el dinero en su saldo total y disponible
                billeteraVendedor.SaldoTotal += monto;
                billeteraVendedor.SaldoDisponible = billeteraVendedor.SaldoTotal - billeteraVendedor.SaldoRetenido;
                _context.Billeteras.Update(billeteraVendedor);

                // Registro en libro mayor (Ledger) para el vendedor: COBRO
                var cobroLedger = new TransaccionLedger
                {
                    BilleteraId = billeteraVendedor.Id,
                    SubastaId = subasta.Id,
                    Tipo = "COBRO",
                    Monto = monto,
                    Fecha = now
                };
                await _context.TransaccionLedger.AddAsync(cobroLedger);

                // 3. Auditoría inmutable de cierre con adjudicación y venta
                var logAuditoria = new AuditoriaLog
                {
                    Entidad = "SUBASTA",
                    EntidadId = subasta.Id,
                    Accion = "CIERRE_WORKER",
                    UsuarioId = null, // Ejecutado automáticamente por el Background Worker
                    Fecha = now,
                    DetalleJson = JsonSerializer.Serialize(new
                    {
                        Estado = "FINALIZADA",
                        GanadorId = ganadorId,
                        VendedorId = vendedorId,
                        MontoLiquidado = monto,
                        FechaCierre = now
                    })
                };
                await _context.AuditoriaLogs.AddAsync(logAuditoria);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task FinalizarSubastaDesiertaAsync(int subastaId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                DateTime now = DateTime.UtcNow;

                var subasta = await _context.Subastas.FirstOrDefaultAsync(s => s.Id == subastaId);
                if (subasta == null) return;

                subasta.Estado = "DESIERTA";
                _context.Subastas.Update(subasta);

                // Auditoría inmutable de cierre desierto
                var logAuditoria = new AuditoriaLog
                {
                    Entidad = "SUBASTA",
                    EntidadId = subasta.Id,
                    Accion = "CIERRE_WORKER",
                    UsuarioId = null,
                    Fecha = now,
                    DetalleJson = JsonSerializer.Serialize(new
                    {
                        Estado = "DESIERTA",
                        Mensaje = "Subasta vencida sin ofertas registradas.",
                        FechaCierre = now
                    })
                };
                await _context.AuditoriaLogs.AddAsync(logAuditoria);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ActivarSubastaProgramadaAsync(int subastaId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                DateTime now = DateTime.UtcNow;

                var subasta = await _context.Subastas.FirstOrDefaultAsync(s => s.Id == subastaId);
                if (subasta == null) return;

                subasta.Estado = "ACTIVA";
                _context.Subastas.Update(subasta);

                var logAuditoria = new AuditoriaLog
                {
                    Entidad = "SUBASTA",
                    EntidadId = subasta.Id,
                    Accion = "INICIO_WORKER",
                    UsuarioId = null,
                    Fecha = now,
                    DetalleJson = JsonSerializer.Serialize(new
                    {
                        Estado = "ACTIVA",
                        Mensaje = "Subasta iniciada automáticamente al llegar su FechaInicio.",
                        FechaInicio = now
                    })
                };
                await _context.AuditoriaLogs.AddAsync(logAuditoria);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
