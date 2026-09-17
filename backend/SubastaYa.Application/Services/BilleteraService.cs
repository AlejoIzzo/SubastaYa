using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Services
{
    public class BilleteraService : IBilleteraService
    {
        private readonly IBilleteraRepository _billeteraRepository;

        public BilleteraService(IBilleteraRepository billeteraRepository)
        {
            _billeteraRepository = billeteraRepository;
        }

        public async Task<BilleteraDTO?> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _billeteraRepository.GetDtoByUsuarioIdAsync(usuarioId);
        }

        public async Task<BilleteraDTO?> GetByIdAsync(int id)
        {
            return await _billeteraRepository.GetDtoByIdAsync(id);
        }

        public async Task<IEnumerable<TransaccionLedgerDTO>> GetTransaccionesAsync(int usuarioId)
        {
            return await _billeteraRepository.GetTransaccionesByUsuarioIdAsync(usuarioId);
        }

        public async Task<PagedResultDTO<TransaccionLedgerDTO>> GetTransaccionesPaginadasAsync(int billeteraId, int pagina = 1, int tamanioPagina = 5)
        {
            return await _billeteraRepository.GetTransaccionesPaginadasByBilleteraIdAsync(billeteraId, pagina, tamanioPagina);
        }
        public async Task<BilleteraDTO> CargarSaldoAsync(int billeteraId, decimal monto)
        {
            if (monto <= 0)
                throw new DominioException("El monto a depositar debe ser mayor a cero");

            Billetera? billetera = await _billeteraRepository.GetByIdAsync(billeteraId);
            if (billetera == null)
                throw new DominioException($"No se encontró la billetera con id ${billeteraId}");

            // Actualización de saldos
            billetera.SaldoTotal += monto;
            billetera.SaldoDisponible += monto;

            // Registro contable
            var transaccion = new TransaccionLedger
            {
                BilleteraId = billetera.Id,
                SubastaId = null,
                Tipo = "DEPOSITO",
                Monto = monto,
                Fecha = DateTime.UtcNow
            };
            await _billeteraRepository.AgregarTransaccionAsync(transaccion);

            // Registro de auditoría
            var logAuditoria = new AuditoriaLog
            {
                Entidad = "BILLETERA",
                EntidadId = billetera.Id,
                Accion = "ACREDITACION_MANUAL",
                UsuarioId = billetera.UsuarioId,
                Fecha = DateTime.UtcNow,
                DetalleJson = JsonSerializer.Serialize(new
                {
                    MontoDepositado = monto,
                    NuevoSaldoTotal = billetera.SaldoTotal,
                    NuevoSaldoDisponible = billetera.SaldoDisponible
                })
            };
            await _billeteraRepository.AgregarAuditoriaAsync(logAuditoria);

            await _billeteraRepository.GuardarCambiosAsync();

            var dto = await _billeteraRepository.GetDtoByUsuarioIdAsync(billetera.UsuarioId);
            return dto!;
        }
    }
}
