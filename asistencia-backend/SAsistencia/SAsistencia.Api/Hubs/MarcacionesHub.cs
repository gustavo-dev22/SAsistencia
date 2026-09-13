using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace SAsistencia.Api.Hubs
{
    public class MarcacionesHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // El cliente se conecta al canal del monitor en vivo
            await Groups.AddToGroupAsync(Context.ConnectionId, "LiveMonitorGroup");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "LiveMonitorGroup");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
