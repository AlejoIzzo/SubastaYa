using Microsoft.AspNetCore.Mvc;
using Moq;
using SubastaYa.API.Controllers;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Interfaces;
using SubastaYa.Application.Services;
using SubastaYa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SubastaYa.Tests;

public class AuditoriaServiceTests
{
    [Fact]
    public async Task GetAuditoriasAsync_MapeaEntidadesADTOsCorrectamente()
    {
        // ARRANGE
        var mockRepo = new Mock<IAuditoriaRepository>();
        var logs = new List<AuditoriaLog>
        {
            new AuditoriaLog
            {
                Id = 1,
                Entidad = "SUBASTA",
                EntidadId = 1,
                Accion = "CREACION_SUBASTA",
                UsuarioId = 1,
                Usuario = new Usuario { Id = 1, Nombre = "Vendedor Test" },
                Fecha = DateTime.UtcNow,
                DetalleJson = "{\"PrecioBase\":30000}"
            }
        };

        mockRepo.Setup(r => r.GetAuditoriasAsync("SUBASTA", "CREACION_SUBASTA", 1, 100))
            .ReturnsAsync(logs);

        var service = new AuditoriaService(mockRepo.Object);

        // ACT
        var result = (await service.GetAuditoriasAsync("SUBASTA", "CREACION_SUBASTA", 1)).ToList();

        // ASSERT
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("SUBASTA", result[0].Entidad);
        Assert.Equal("CREACION_SUBASTA", result[0].Accion);
        Assert.Equal("Vendedor Test", result[0].UsuarioNombre);
    }

    [Fact]
    public async Task GetAuditoriasBySubastaAsync_MapeaEntidadesADTOsCorrectamente()
    {
        // ARRANGE
        var mockRepo = new Mock<IAuditoriaRepository>();
        var logs = new List<AuditoriaLog>
        {
            new AuditoriaLog
            {
                Id = 10,
                Entidad = "SUBASTA",
                EntidadId = 2,
                Accion = "EXTENSION_TIEMPO",
                UsuarioId = 2,
                Usuario = new Usuario { Id = 2, Nombre = "Comprador 1" },
                Fecha = DateTime.UtcNow,
                DetalleJson = "{\"ExtensionMinutos\":2}"
            }
        };

        mockRepo.Setup(r => r.GetAuditoriasBySubastaAsync(2))
            .ReturnsAsync(logs);

        var service = new AuditoriaService(mockRepo.Object);

        // ACT
        var result = (await service.GetAuditoriasBySubastaAsync(2)).ToList();

        // ASSERT
        Assert.Single(result);
        Assert.Equal(10, result[0].Id);
        Assert.Equal("EXTENSION_TIEMPO", result[0].Accion);
        Assert.Equal(2, result[0].EntidadId);
    }

    [Fact]
    public async Task AuditoriasController_DelegaEnAuditoriaService()
    {
        // ARRANGE
        var mockService = new Mock<IAuditoriaService>();
        var dtos = new List<AuditoriaLogDTO>
        {
            new AuditoriaLogDTO
            {
                Id = 1,
                Entidad = "SUBASTA",
                EntidadId = 1,
                Accion = "CREACION_SUBASTA"
            }
        };

        mockService.Setup(s => s.GetAuditoriasAsync(null, null, null))
            .ReturnsAsync(dtos);

        var controller = new AuditoriasController(mockService.Object);

        // ACT
        var actionResult = await controller.GetAuditorias();

        // ASSERT
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedLogs = Assert.IsAssignableFrom<IEnumerable<AuditoriaLogDTO>>(okResult.Value);
        Assert.Single(returnedLogs);
    }
}
