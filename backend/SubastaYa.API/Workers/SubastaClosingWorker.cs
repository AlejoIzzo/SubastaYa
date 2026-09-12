using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubastaYa.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SubastaYa.API.Workers
{
    public class SubastaClosingWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubastaClosingWorker> _logger;

        public SubastaClosingWorker(IServiceProvider serviceProvider, ILogger<SubastaClosingWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
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

                    int cerradas = await closingService.ProcesarSubastasVencidasAsync();
                    if (cerradas > 0)
                    {
                        _logger.LogInformation("[SubastaClosingWorker] Se procesaron y liquidaron {Count} subastas vencidas.", cerradas);
                    }

                    int iniciadas = await closingService.IniciarSubastasProgramadasAsync();
                    if (iniciadas > 0)
                    {
                        _logger.LogInformation("[SubastaClosingWorker] Se activaron {Count} subastas programadas.", iniciadas);
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
