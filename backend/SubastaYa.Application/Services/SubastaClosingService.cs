using SubastaYa.Application.Interfaces;
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

        public async Task<int> ProcesarSubastasVencidasAsync()
        {
            var vencidas = await _closingRepository.GetSubastasVencidasAsync();
            int procesadas = 0;

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

                procesadas++;
            }

            return procesadas;
        }

        public async Task<int> IniciarSubastasProgramadasAsync()
        {
            var programadas = await _closingRepository.GetSubastasProgramadasParaIniciarAsync();
            int iniciadas = 0;

            foreach (var subasta in programadas)
            {
                await _closingRepository.ActivarSubastaProgramadaAsync(subasta.Id);
                iniciadas++;
            }

            return iniciadas;
        }
    }
}
