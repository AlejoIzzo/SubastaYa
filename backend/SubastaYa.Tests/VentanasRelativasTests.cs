using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Services;
using SubastaYa.Data;
using SubastaYa.Infrastructure.Data;
using SubastaYa.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SubastaYa.Tests;

[Collection("DatabaseTests")]
public class VentanasRelativasTests : IDisposable
{
    private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=SubastaYaTestDB_Ventanas;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public VentanasRelativasTests()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
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
    public async Task InicializarVentanasRelativas_ConfiguraSubastasConTiemposRelativosCorrectos()
    {
        // ARRANGE:
        using var context = CreateContext();

        // ACT:
        await DbInitializer.InicializarVentanasRelativasAsync(context);

        // ASSERT:
        var subasta1 = await context.Subastas.FirstAsync(s => s.Id == 1);
        var subasta2 = await context.Subastas.FirstAsync(s => s.Id == 2);
        var subasta3 = await context.Subastas.FirstAsync(s => s.Id == 3);

        var now = DateTime.UtcNow;

        // Subasta 1 (Activa estándar): Cierra en ventana corta de ~25 min
        Assert.Equal("ACTIVA", subasta1.Estado);
        var restanteSubasta1 = subasta1.FechaFin - now;
        Assert.True(restanteSubasta1.TotalMinutes >= 20 && restanteSubasta1.TotalMinutes <= 30,
            $"Subasta 1 debe cerrar entre 20 y 30 min (actual: {restanteSubasta1.TotalMinutes:N1} min)");

        // Subasta 2 (Activa crítica): Cierra en menos de 2 min (~90 seg)
        Assert.Equal("ACTIVA", subasta2.Estado);
        var restanteSubasta2 = subasta2.FechaFin - now;
        Assert.True(restanteSubasta2.TotalSeconds > 60 && restanteSubasta2.TotalSeconds <= 120,
            $"Subasta 2 debe cerrar en menos de 2 min y más de 60 seg (actual: {restanteSubasta2.TotalSeconds:N1} seg)");

        // Subasta 3 (Próxima): Programada a +24 hs
        Assert.Equal("PROGRAMADA", subasta3.Estado);
        var inicioSubasta3 = subasta3.FechaInicio - now;
        Assert.True(inicioSubasta3.TotalHours >= 23 && inicioSubasta3.TotalHours <= 25,
            $"Subasta 3 debe iniciar en aproximadamente +24 hs (actual: {inicioSubasta3.TotalHours:N1} hs)");
    }

    [Fact]
    public async Task PujaEnZonaCritica_Subasta2_DisparaAntiSniping_YAuditaExtension()
    {
        // ARRANGE:
        using var context = CreateContext();
        await DbInitializer.InicializarVentanasRelativasAsync(context);

        // Aseguramos que la subasta 2 esté en los últimos 45 segundos para probar el anti-sniping (< 60s)
        var subasta2 = await context.Subastas.FirstAsync(s => s.Id == 2);
        var fechaFinOriginal = DateTime.UtcNow.AddSeconds(45);
        subasta2.FechaFin = fechaFinOriginal;
        await context.SaveChangesAsync();

        var pujaRepo = new PujaRepository(context);
        var subastaRepo = new SubastaRepository(context);
        var billeteraRepo = new BilleteraRepository(context);
        var usuarioRepo = new UsuarioRepository(context);
        var pujaService = new PujaService(pujaRepo, subastaRepo, billeteraRepo, usuarioRepo);

        // Comprador 2 oferta sobre la Carta Pokémon ($10.000 precio base)
        var dto = new CrearPujaDTO
        {
            CompradorId = 3,
            Monto = 10000.00m
        };

        // ACT:
        var resultado = await pujaService.CrearPujaAsync(2, dto);

        // ASSERT:
        Assert.NotNull(resultado);
        Assert.True(resultado.AntiSnipingActivado, "El anti-sniping debió activarse al ofertar con < 60s restantes");

        // La nueva fecha de fin debe haberse extendido por 2 minutos
        var subastaActualizada = await context.Subastas.FirstAsync(s => s.Id == 2);
        Assert.True(subastaActualizada.FechaFin >= fechaFinOriginal.AddMinutes(1.9),
            "La fecha de fin debió extenderse al menos 2 minutos");

        // Debe registrarse en la tabla de auditoría con la acción EXTENSION_TIEMPO
        var auditLog = await context.AuditoriaLogs
            .FirstOrDefaultAsync(a => a.EntidadId == 2 && a.Accion == "EXTENSION_TIEMPO");

        Assert.NotNull(auditLog);
        Assert.Equal("SUBASTA", auditLog.Entidad);
    }
}
