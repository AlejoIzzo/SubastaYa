using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Interfaces;
using SubastaYa.Application.Services;
using SubastaYa.Data;
using SubastaYa.Domain.Entities;
using SubastaYa.Infrastructure.Data;
using SubastaYa.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SubastaYa.Tests;

[Collection("DatabaseTests")]
public class LiquidacionSaldoRetenidoTests : IDisposable
{
    private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=SubastaYaTestDB_Liquidacion;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public LiquidacionSaldoRetenidoTests()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        DbInitializer.InicializarVentanasRelativasAsync(context).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
    }

    private SubastaYaContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SubastaYaContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new SubastaYaContext(options);
    }

    [Fact]
    public async Task Liquidacion_DescuentaSoloRetencionDeSubastaLiquidada_ConservaSaldoRetenidoDeOtras()
    {
        // ARRANGE:
        // En el seed, Comprador 1 (UsuarioId: 2) tiene $135.000 retenidos:
        // - $45.000 retenidos por Subasta 1 (líder activo)
        // - $90.000 retenidos por Subasta 4 (ganador por liquidar)
        // SaldoTotal: $240.000, SaldoDisponible: $105.000
        using var context = CreateContext();
        var closingRepo = new SubastaClosingRepository(context);

        // ACT:
        // Liquidar Subasta 4 con ganador Comprador 1 ($90.000)
        await closingRepo.FinalizarSubastaConGanadorAsync(
            subastaId: 4,
            ganadorId: 2,
            vendedorId: 1,
            monto: 90000.00m
        );

        // ASSERT:
        var billeteraComprador = await context.Billeteras.FirstAsync(b => b.UsuarioId == 2);
        var billeteraVendedor = await context.Billeteras.FirstAsync(b => b.UsuarioId == 1);
        var subasta4 = await context.Subastas.FirstAsync(s => s.Id == 4);

        // Subasta debe estar FINALIZADA
        Assert.Equal("FINALIZADA", subasta4.Estado);

        // El comprador DEBE conservar exactamente $45.000 de saldo retenido (correspondiente a Subasta 1)
        Assert.Equal(45000.00m, billeteraComprador.SaldoRetenido);
        Assert.True(billeteraComprador.SaldoRetenido >= 0, "SaldoRetenido no debe ser negativo");

        // Saldo total debe descontar los $90.000 pagados: 240.000 - 90.000 = 150.000
        Assert.Equal(150000.00m, billeteraComprador.SaldoTotal);

        // Saldo disponible debe mantenerse en $105.000 (150.000 - 45.000)
        Assert.Equal(105000.00m, billeteraComprador.SaldoDisponible);
        Assert.True(billeteraComprador.SaldoDisponible >= 0, "SaldoDisponible no debe ser negativo");

        // El vendedor debe recibir $90.000 en su saldo total y disponible
        Assert.Equal(90000.00m, billeteraVendedor.SaldoTotal);
        Assert.Equal(90000.00m, billeteraVendedor.SaldoDisponible);
        Assert.Equal(0.00m, billeteraVendedor.SaldoRetenido);

        // Comprobar transacciones en el Ledger
        var pagoComprador = await context.TransaccionLedger.FirstOrDefaultAsync(t => t.BilleteraId == billeteraComprador.Id && t.SubastaId == 4 && t.Tipo == "PAGO");
        var cobroVendedor = await context.TransaccionLedger.FirstOrDefaultAsync(t => t.BilleteraId == billeteraVendedor.Id && t.SubastaId == 4 && t.Tipo == "COBRO");

        Assert.NotNull(pagoComprador);
        Assert.Equal(90000.00m, pagoComprador.Monto);
        Assert.NotNull(cobroVendedor);
        Assert.Equal(90000.00m, cobroVendedor.Monto);
    }

    [Fact]
    public async Task SuperacionDePuja_TrasLiquidacion_DejaSaldoRetenidoEnCero_YNoNegativo()
    {
        // ARRANGE:
        using var context = CreateContext();
        var closingRepo = new SubastaClosingRepository(context);

        // 1. El worker liquida Subasta 4
        await closingRepo.FinalizarSubastaConGanadorAsync(
            subastaId: 4,
            ganadorId: 2,
            vendedorId: 1,
            monto: 90000.00m
        );

        // Verificar estado intermedio: Comprador 1 conserva sus $45.000 retenidos de Subasta 1
        var billeteraComprador1 = await context.Billeteras.FirstAsync(b => b.UsuarioId == 2);
        Assert.Equal(45000.00m, billeteraComprador1.SaldoRetenido);

        // 2. Comprador 2 (UsuarioId: 3) supera la oferta de Comprador 1 en Subasta 1 con $50.000
        var pujaRepo = new PujaRepository(context);
        var subastaRepo = new SubastaRepository(context);
        var billeteraRepo = new BilleteraRepository(context);
        var usuarioRepo = new UsuarioRepository(context);
        var pujaService = new PujaService(pujaRepo, subastaRepo, billeteraRepo, usuarioRepo);

        var dtoNuevaPuja = new CrearPujaDTO
        {
            CompradorId = 3,
            Monto = 50000.00m
        };

        // ACT:
        var resultado = await pujaService.CrearPujaAsync(1, dtoNuevaPuja);
        Assert.NotNull(resultado);

        // ASSERT:
        // Recargar Comprador 1
        using var contextVerification = CreateContext();
        var bComprador1 = await contextVerification.Billeteras.FirstAsync(b => b.UsuarioId == 2);
        var bComprador2 = await contextVerification.Billeteras.FirstAsync(b => b.UsuarioId == 3);

        // Comprador 1 debe quedar con exactamente $0.00 retenido (NUNCA negativo)
        Assert.Equal(0.00m, bComprador1.SaldoRetenido);
        Assert.True(bComprador1.SaldoRetenido >= 0, "SaldoRetenido no debe ser negativo bajo ninguna circunstancia");
        Assert.Equal(150000.00m, bComprador1.SaldoTotal);
        Assert.Equal(150000.00m, bComprador1.SaldoDisponible);

        // Comprador 2 es el nuevo líder con $50.000 retenidos
        Assert.Equal(50000.00m, bComprador2.SaldoRetenido);
        Assert.Equal(200000.00m, bComprador2.SaldoTotal);
        Assert.Equal(150000.00m, bComprador2.SaldoDisponible);
    }

    [Fact]
    public async Task Liquidacion_SaldoRetenidoInsuficiente_LanzaExcepcion_YNoTruncaConMathMax()
    {
        // ARRANGE:
        using var context = CreateContext();
        // Simulamos una alteración inconsistente donde la billetera del comprador tiene menos retenido que el ledger
        var billetera = await context.Billeteras.FirstAsync(b => b.UsuarioId == 2);
        billetera.SaldoRetenido = 30000.00m; // Menor a los $90.000 requeridos por Subasta 4
        await context.SaveChangesAsync();

        var closingRepo = new SubastaClosingRepository(context);

        // ACT & ASSERT:
        // Debe lanzar InvalidOperationException en lugar de truncar silenciosamente a 0 con Math.Max
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            closingRepo.FinalizarSubastaConGanadorAsync(subastaId: 4, ganadorId: 2, vendedorId: 1, monto: 90000.00m)
        );

        Assert.Contains("saldo retenido", ex.Message.ToLower());
    }

    [Fact]
    public async Task Liquidacion_SaldoTotalInsuficiente_LanzaExcepcion()
    {
        // ARRANGE:
        using var context = CreateContext();
        var billetera = await context.Billeteras.FirstAsync(b => b.UsuarioId == 2);
        billetera.SaldoTotal = 50000.00m; // Menor a los $90.000 a liquidar
        await context.SaveChangesAsync();

        var closingRepo = new SubastaClosingRepository(context);

        // ACT & ASSERT:
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            closingRepo.FinalizarSubastaConGanadorAsync(subastaId: 4, ganadorId: 2, vendedorId: 1, monto: 90000.00m)
        );

        Assert.Contains("saldo total", ex.Message.ToLower());
    }

    [Fact]
    public async Task CheckConstraints_BaseDeDatos_RechazaSaldoRetenidoNegativo()
    {
        // ARRANGE:
        using var context = CreateContext();
        var billetera = await context.Billeteras.FirstAsync(b => b.UsuarioId == 2);

        // ACT: Forzar saldo retenido negativo directamente
        billetera.SaldoRetenido = -1000.00m;

        // ASSERT: La base de datos relacional debe rechazar la operación mediante el CHECK constraint
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }
}
