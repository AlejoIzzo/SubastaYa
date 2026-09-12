using System.Threading.Tasks;

namespace SubastaYa.Application.Interfaces
{
    public interface ISubastaClosingService
    {
        Task<int> ProcesarSubastasVencidasAsync();
        Task<int> IniciarSubastasProgramadasAsync();
    }
}
