using Microsoft.AspNetCore.SignalR;
using SAsistencia.Api.Hubs;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Marcaciones.DTOs;

namespace SAsistencia.Api.Services;

public class MarcacionNotifier : IMarcacionNotifier
{
    private readonly IHubContext<MarcacionesHub> _hubContext;

    public MarcacionNotifier(IHubContext<MarcacionesHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotificarNuevaMarcacionAsync(MarcacionEnVivoDto marcacion, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group("LiveMonitorGroup")
            .SendAsync("RecibirMarcacionEnVivo", marcacion, cancellationToken);
    }
}