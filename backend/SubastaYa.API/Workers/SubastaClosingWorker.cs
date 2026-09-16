using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubastaYa.API.Hubs;
using SubastaYa.Application.Interfaces;
using SubastaYa.Infrastructure.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SubastaYa.API.Workers
{
    public class SubastaClosingWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubastaClosingWorker> _logger;
        private readonly IHubContext<SubastaHub> _hubContext;

        public SubastaClosingWorker(IServiceProvider serviceProvider, ILogger<SubastaClosingWorker> logger, IHubContext<SubastaHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[SubastaClosingWorker] Worker en segundo plano iniciado exitosamente.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var closingService = scope.ServiceProvider.GetRequiredService<ISubastaClosingService>();
                    var subastaRepository = scope.ServiceProvider.GetRequiredService<ISubastaRepository>();

                    // 1. Para subastas finalizadas / desiertas:
                    var idsCerradas = await closingService.ProcesarSubastasVencidasAsync();
                    if (idsCerradas.Count > 0)
                    {
                        _logger.LogInformation("[SubastaClosingWorker] Se procesaron y liquidaron {Count} subastas vencidas.", idsCerradas.Count);

                        foreach (var subastaId in idsCerradas)
                        {
                            var detalle = await subastaRepository.GetByIdAsync(subastaId);
                            if (detalle != null)
                            {
                                // Se emite al grupo de la subasta en vivo y a todos los clientes
                                await _hubContext.Clients.Group($"Subasta-{subastaId}").SendAsync("SubastaFinalizada", detalle);
                                await _hubContext.Clients.All.SendAsync("SubastaFinalizada", detalle);
                            }
                        }
                    }

                    // 2. Para subastas que acaban de iniciar (pasan de PROGRAMADA a ACTIVA):
                    var idsIniciadas = await closingService.IniciarSubastasProgramadasAsync();
                    if (idsIniciadas.Count > 0)
                    {
                        _logger.LogInformation("[SubastaClosingWorker] Se activaron {Count} subastas programadas.", idsIniciadas.Count);

                        foreach (var subastaId in idsIniciadas)
                        {
                            var detalle = await subastaRepository.GetByIdAsync(subastaId);
                            if (detalle != null)
                            {
                                // Se emite al grupo de la subasta en vivo y a todos los clientes
                                await _hubContext.Clients.Group($"Subasta-{subastaId}").SendAsync("SubastaIniciada", detalle);
                                await _hubContext.Clients.All.SendAsync("SubastaIniciada", detalle);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[SubastaClosingWorker] Error inesperado en el ciclo del worker.");
                }

                // Espera 10 segundos antes del siguiente ciclo
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
