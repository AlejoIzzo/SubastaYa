using System.Threading.Tasks;

namespace SubastaYa.Application.Interfaces
{
    public interface ISubastaClosingService
    {
        Task<System.Collections.Generic.List<int>> ProcesarSubastasVencidasAsync();
        Task<System.Collections.Generic.List<int>> IniciarSubastasProgramadasAsync();
    }
}
