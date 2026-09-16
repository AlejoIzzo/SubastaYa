using SubastaYa.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubastaYa.Application.Services
{
    public class SubastaClosingService : ISubastaClosingService
    {
        private readonly ISubastaClosingRepository _closingRepository;

        public SubastaClosingService(ISubastaClosingRepository closingRepository)
        {
            _closingRepository = closingRepository;
        }

        public async Task<List<int>> ProcesarSubastasVencidasAsync()
        {
            var vencidas = await _closingRepository.GetSubastasVencidasAsync();
            var idsProcesadas = new List<int>();

            foreach (var subasta in vencidas)
            {
                if (subasta.Pujas != null && subasta.Pujas.Any())
                {
                    // Obtener la puja ganadora con el monto más alto
                    var pujaGanadora = subasta.Pujas.OrderByDescending(p => p.Monto).First();

                    await _closingRepository.FinalizarSubastaConGanadorAsync(
                        subasta.Id,
                        pujaGanadora.CompradorId,
                        subasta.VendedorId,
                        pujaGanadora.Monto
                    );
                }
                else
                {
                    // Si no hubo pujas, la subasta se declara desierta
                    await _closingRepository.FinalizarSubastaDesiertaAsync(subasta.Id);
                }

                idsProcesadas.Add(subasta.Id);
            }

            return idsProcesadas;
        }

        public async Task<List<int>> IniciarSubastasProgramadasAsync()
        {
            var programadas = await _closingRepository.GetSubastasProgramadasParaIniciarAsync();
            var idsIniciadas = new List<int>();

            foreach (var subasta in programadas)
            {
                await _closingRepository.ActivarSubastaProgramadaAsync(subasta.Id);
                idsIniciadas.Add(subasta.Id);
            }

            return idsIniciadas;
        }
    }
}
