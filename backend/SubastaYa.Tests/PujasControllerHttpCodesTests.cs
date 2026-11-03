using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using SubastaYa.API.Controllers;
using SubastaYa.API.Hubs;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.Services;
using SubastaYa.Data;
using SubastaYa.Infrastructure.Data;
using SubastaYa.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SubastaYa.Tests;

[Collection("DatabaseTests")]
public class PujasControllerHttpCodesTests : IDisposable
{
    private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=SubastaYaTestDB_HttpCodes;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public PujasControllerHttpCodesTests()
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
    public async Task CrearPuja_UsuarioSinFondos_Retorna422UnprocessableEntity_RUN05()
    {
        // ARRANGE:
        using var context = CreateContext();
        var pujaRepo = new PujaRepository(context);
        var subastaRepo = new SubastaRepository(context);
        var billeteraRepo = new BilleteraRepository(context);
        var usuarioRepo = new UsuarioRepository(context);
        var pujaService = new PujaService(pujaRepo, subastaRepo, billeteraRepo, usuarioRepo);

        var mockHubContext = new Mock<IHubContext<SubastaHub>>();
        var controller = new PujasController(pujaService, mockHubContext.Object);

        // Usuario 4 ("Sin Fondos", dispone de $500) intenta pujar $50.000 en Subasta 1
        var dto = new CrearPujaDTO
        {
            CompradorId = 4,
            Monto = 50000.00m
        };

        // ACT:
        var actionResult = await controller.CrearPuja(1, dto);

        // ASSERT:
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, objectResult.StatusCode);

        // Verificar que el payload contiene status: 422 y error: Unprocessable Entity
        dynamic body = objectResult.Value!;
        var bodyType = body.GetType();
        int status = (int)bodyType.GetProperty("status")!.GetValue(body, null)!;
        string error = (string)bodyType.GetProperty("error")!.GetValue(body, null)!;
        string message = (string)bodyType.GetProperty("message")!.GetValue(body, null)!;

        Assert.Equal(422, status);
        Assert.Equal("Unprocessable Entity", error);
        Assert.Contains("saldo disponible insuficiente", message.ToLower());
    }

    [Fact]
    public async Task CrearPuja_MontoInferiorAlMinimo_Retorna400BadRequest_RUN06()
    {
        // ARRANGE:
        using var context = CreateContext();
        var pujaRepo = new PujaRepository(context);
        var subastaRepo = new SubastaRepository(context);
        var billeteraRepo = new BilleteraRepository(context);
        var usuarioRepo = new UsuarioRepository(context);
        var pujaService = new PujaService(pujaRepo, subastaRepo, billeteraRepo, usuarioRepo);

        var mockHubContext = new Mock<IHubContext<SubastaHub>>();
        var controller = new PujasController(pujaService, mockHubContext.Object);

        // Comprador 2 (tiene saldo) intenta pujar $30.000, pero la subasta 1 tiene líder en $45.000 + inc $5.000 = $50.000 mín
        var dto = new CrearPujaDTO
        {
            CompradorId = 3,
            Monto = 30000.00m
        };

        // ACT:
        var actionResult = await controller.CrearPuja(1, dto);

        // ASSERT:
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        dynamic body = badRequestResult.Value!;
        var bodyType = body.GetType();
        int status = (int)bodyType.GetProperty("status")!.GetValue(body, null)!;
        string error = (string)bodyType.GetProperty("error")!.GetValue(body, null)!;
        string message = (string)bodyType.GetProperty("message")!.GetValue(body, null)!;

        Assert.Equal(400, status);
        Assert.Equal("Bad Request", error);
        Assert.Contains("monto insuficiente", message.ToLower());
    }

    [Fact]
    public async Task CrearPuja_SubastaInexistente_Retorna404NotFound()
    {
        // ARRANGE:
        using var context = CreateContext();
        var pujaRepo = new PujaRepository(context);
        var subastaRepo = new SubastaRepository(context);
        var billeteraRepo = new BilleteraRepository(context);
        var usuarioRepo = new UsuarioRepository(context);
        var pujaService = new PujaService(pujaRepo, subastaRepo, billeteraRepo, usuarioRepo);

        var mockHubContext = new Mock<IHubContext<SubastaHub>>();
        var controller = new PujasController(pujaService, mockHubContext.Object);

        var dto = new CrearPujaDTO
        {
            CompradorId = 3,
            Monto = 50000.00m
        };

        // ACT:
        var actionResult = await controller.CrearPuja(9999, dto);

        // ASSERT:
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

        dynamic body = notFoundResult.Value!;
        var bodyType = body.GetType();
        int status = (int)bodyType.GetProperty("status")!.GetValue(body, null)!;
        string error = (string)bodyType.GetProperty("error")!.GetValue(body, null)!;

        Assert.Equal(404, status);
        Assert.Equal("Not Found", error);
    }

    [Fact]
    public async Task CrearPuja_VendedorOfertaEnSuPropiaSubasta_Retorna400BadRequest_BIZ12()
    {
        // ARRANGE:
        // En el seed, Subasta 1 tiene VendedorId = 1
        using var context = CreateContext();
        var pujaRepo = new PujaRepository(context);
        var subastaRepo = new SubastaRepository(context);
        var billeteraRepo = new BilleteraRepository(context);
        var usuarioRepo = new UsuarioRepository(context);
        var pujaService = new PujaService(pujaRepo, subastaRepo, billeteraRepo, usuarioRepo);

        var mockHubContext = new Mock<IHubContext<SubastaHub>>();
        var controller = new PujasController(pujaService, mockHubContext.Object);

        var dto = new CrearPujaDTO
        {
            CompradorId = 1, // Vendedor de la subasta 1
            Monto = 50000.00m
        };

        // ACT:
        var actionResult = await controller.CrearPuja(1, dto);

        // ASSERT:
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        dynamic body = badRequestResult.Value!;
        var bodyType = body.GetType();
        int status = (int)bodyType.GetProperty("status")!.GetValue(body, null)!;
        string error = (string)bodyType.GetProperty("error")!.GetValue(body, null)!;
        string message = (string)bodyType.GetProperty("message")!.GetValue(body, null)!;

        Assert.Equal(400, status);
        Assert.Equal("Bad Request", error);
        Assert.Contains("vendedor no puede ofertar", message.ToLower());
    }
}
