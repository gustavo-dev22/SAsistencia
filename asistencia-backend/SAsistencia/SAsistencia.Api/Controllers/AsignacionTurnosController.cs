using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.Turnos.DTOs;
using SAsistencia.Application.Features.Turnos.Commands;
using SAsistencia.Application.Features.Empleados.Queries;

namespace SAsistencia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AsignacionTurnosController : ControllerBase
{
    private readonly ISender _mediator;

    public AsignacionTurnosController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("masiva")]
    public async Task<ActionResult> AsignarTurnoMasivo([FromBody] AsignarTurnoMasivoRequest request)
    {
        var total = await _mediator.Send(new AsignarTurnoMasivoCommand(request));
        return Ok(new { mensaje = $"Se actualizó el turno a {total} colaborador(es) correctamente." });
    }

    [HttpPost("por-oficina")]
    public async Task<ActionResult> AsignarTurnoPorOficina([FromBody] AsignarTurnoPorOficinaRequest request)
    {
        await _mediator.Send(new AsignarTurnoPorOficinaCommand(request));
        return Ok(new { mensaje = "Turno asignado a toda el área de forma exitosa." });
    }
}
