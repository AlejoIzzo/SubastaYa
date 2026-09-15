using Microsoft.AspNetCore.SignalR;

namespace SubastaYa.API.Hubs
{
    public class SubastaHub : Hub
    {
        // Unirse a una subasta es como unirse a una sala
        public async Task UnirseASubasta(int subastaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Subasta-{subastaId}");
        }

        // Cuando el usuario sale de la pantalla, abandona la sala
        public async Task SalirDeSubasta(int subastaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Subasta-{subastaId}");
        }
        //Groups "filtra" y nos actualiza solo de lo que estamos viendo
    }
}