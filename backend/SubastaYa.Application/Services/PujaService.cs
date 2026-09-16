using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Services
{
    public class PujaService : IPujaService
    {
        private readonly IPujaRepository _pujaRepository;
        private readonly ISubastaRepository _subastaRepository;
        private readonly IBilleteraRepository _billeteraRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public PujaService(
            IPujaRepository pujaRepository,
            ISubastaRepository subastaRepository,
            IBilleteraRepository billeteraRepository,
            IUsuarioRepository usuarioRepository)
        {
            _pujaRepository = pujaRepository;
            _subastaRepository = subastaRepository;
            _billeteraRepository = billeteraRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<PujaDTO>> GetPujasBySubastaIdAsync(int subastaId)
        {
            return await _pujaRepository.GetPujasBySubastaIdAsync(subastaId);
        }

        public async Task<IEnumerable<PujaDTO>> GetAllPujasAsync(int? usuarioId = null)
        {
            return await _pujaRepository.GetAllPujasAsync(usuarioId);
        }

        public async Task<PujaResultadoDTO> CrearPujaAsync(int subastaId, CrearPujaDTO dto)
        {
            //Validaciones de existencia y estado de la Subasta
            var subasta = await _pujaRepository.GetSubastaParaPujarAsync(subastaId);
            if (subasta == null)
                throw new DominioException($"Subasta con ID {subastaId} no encontrada.");

            if (subasta.Estado != "ACTIVA")
                throw new DominioException($"No se puede ofertar en una subasta que no esté ACTIVA (Estado actual: {subasta.Estado}).");

            DateTime now = DateTime.UtcNow;

            if (now < subasta.FechaInicio)
                throw new DominioException("La subasta aún no ha comenzado.");

            if (now >= subasta.FechaFin)
                throw new DominioException("La subasta ya ha finalizado.");

            //Validaciones del Usuario Comprador
            var comprador = await _usuarioRepository.GetByIdAsync(dto.CompradorId);
            if (comprador == null)
                throw new DominioException($"Usuario comprador con ID {dto.CompradorId} no encontrado.");

            if (subasta.VendedorId == dto.CompradorId)
                throw new DominioException("El vendedor no puede ofertar en su propia subasta.");

            // Determinamos la puja líder actual (si existe)
            var pujaLiderActual = subasta.Pujas
                .OrderByDescending(p => p.Monto)
                .FirstOrDefault();

            if (pujaLiderActual != null && pujaLiderActual.CompradorId == dto.CompradorId)
                throw new DominioException("Ya eres el postor líder con la oferta más alta. No puedes ofertar contra ti mismo.");

            //Validación del Monto Ofertado
            decimal montoMinimoRequerido = pujaLiderActual != null
                ? pujaLiderActual.Monto + subasta.IncrementoMinimo
                : subasta.PrecioBase;

            if (dto.Monto < montoMinimoRequerido)
            {
                throw new DominioException(
                    $"Monto insuficiente. La oferta mínima permitida es de ${montoMinimoRequerido:N2} " +
                    $"(${(pujaLiderActual != null ? pujaLiderActual.Monto : subasta.PrecioBase):N2} + incremento de ${subasta.IncrementoMinimo:N2})."
                );
            }

            //Validación de Fondos Disponibles en la Billetera
            var billeteraComprador = await _billeteraRepository.GetByUsuarioIdAsync(dto.CompradorId);
            if (billeteraComprador == null)
                throw new DominioException("El comprador no posee una billetera registrada en el sistema.");

            if (billeteraComprador.SaldoDisponible < dto.Monto)
            {
                throw new DominioException(
                    $"Saldo disponible insuficiente. Dispones de ${billeteraComprador.SaldoDisponible:N2} " +
                    $"y la puja requiere congelar ${dto.Monto:N2}."
                );
            }

            //Traspaso Atómico de Saldos
            //Congelar el saldo al nuevo postor líder
            billeteraComprador.SaldoDisponible -= dto.Monto;
            billeteraComprador.SaldoRetenido += dto.Monto;

            var transaccionRetencion = new TransaccionLedger
            {
                BilleteraId = billeteraComprador.Id,
                SubastaId = subasta.Id,
                Tipo = "RETENCION",
                Monto = dto.Monto,
                Fecha = now
            };

            //Si había un postor líder anterior, liberar su saldo de forma automática e inmediata
            Billetera? billeteraAnterior = null;
            TransaccionLedger? transaccionLiberacion = null;

            if (pujaLiderActual != null)
            {
                billeteraAnterior = await _billeteraRepository.GetByUsuarioIdAsync(pujaLiderActual.CompradorId);
                if (billeteraAnterior != null)
                {
                    billeteraAnterior.SaldoRetenido -= pujaLiderActual.Monto;
                    billeteraAnterior.SaldoDisponible += pujaLiderActual.Monto;

                    transaccionLiberacion = new TransaccionLedger
                    {
                        BilleteraId = billeteraAnterior.Id,
                        SubastaId = subasta.Id,
                        Tipo = "LIBERACION",
                        Monto = pujaLiderActual.Monto,
                        Fecha = now
                    };
                }
            }

            //Crear la nueva oferta
            var nuevaPuja = new Puja
            {
                SubastaId = subasta.Id,
                CompradorId = dto.CompradorId,
                Monto = dto.Monto,
                Fecha = now
            };

            //Extensión Dinámica de Tiempo
            // Si la oferta entra en los últimos 60 segundos antes del cierre, sumar 2 minutos
            bool antiSnipingActivado = false;
            AuditoriaLog? logAntiSniping = null;
            TimeSpan tiempoRestante = subasta.FechaFin - now;

            if (tiempoRestante <= TimeSpan.FromSeconds(60))
            {
                antiSnipingActivado = true;
                DateTime fechaFinAnterior = subasta.FechaFin;
                subasta.FechaFin = subasta.FechaFin.AddMinutes(2);

                logAntiSniping = new AuditoriaLog
                {
                    Entidad = "SUBASTA",
                    EntidadId = subasta.Id,
                    Accion = "EXTENSION_TIEMPO",
                    UsuarioId = dto.CompradorId,
                    Fecha = DateTime.SpecifyKind(now, DateTimeKind.Utc),
                    DetalleJson = JsonSerializer.Serialize(new
                    {
                        Motivo = "Regla Anti-Sniping activada",
                        SegundosRestantesAlOfertar = Math.Round(tiempoRestante.TotalSeconds, 1),
                        FechaFinAnterior = fechaFinAnterior,
                        NuevaFechaFin = DateTime.SpecifyKind(subasta.FechaFin, DateTimeKind.Utc),
                        MontoOfertado = dto.Monto
                    })
                };
            }

            // 1. Manejo de Billeteras y movimientos en Ledger a través de IBilleteraRepository
            if (billeteraAnterior != null && transaccionLiberacion != null)
            {
                await _billeteraRepository.AgregarTransaccionAsync(transaccionLiberacion);
                _billeteraRepository.Update(billeteraAnterior);
            }

            await _billeteraRepository.AgregarTransaccionAsync(transaccionRetencion);
            _billeteraRepository.Update(billeteraComprador);

            // 2. Manejo de la Subasta a través de ISubastaRepository
            _subastaRepository.Update(subasta);

            // 3. Manejo de Auditoría (si hubo anti-sniping) a través de IBilleteraRepository
            if (logAntiSniping != null)
            {
                await _billeteraRepository.AgregarAuditoriaAsync(logAntiSniping);
            }

            // 4. Manejo de la Puja a través de IPujaRepository
            await _pujaRepository.AgregarAsync(nuevaPuja);

            // 5. Commit atómico en la base de datos (manejando ConcurrenciaException)
            await _pujaRepository.GuardarCambiosAsync();

            //Construir y retornar el resultado

            SubastaDetalleDTO subastaDetalle = await _subastaRepository.GetByIdAsync(subasta.Id);
            
            return new PujaResultadoDTO
            {
                Id = nuevaPuja.Id,
                SubastaId = subasta.Id,
                CompradorId = dto.CompradorId,
                CompradorNombre = comprador.Nombre,
                Monto = nuevaPuja.Monto,
                Fecha = DateTime.SpecifyKind(nuevaPuja.Fecha, DateTimeKind.Utc),
                AntiSnipingActivado = antiSnipingActivado,
                FechaFinSubasta = DateTime.SpecifyKind(subasta.FechaFin, DateTimeKind.Utc),
                Mensaje = antiSnipingActivado
                    ? "Puja líder registrada con éxito. ¡Se extendió el tiempo de la subasta por 2 minutos!"
                    : "Puja líder registrada con éxito.",
                SubastaDetalle = subastaDetalle
            };
        }
    }
}
