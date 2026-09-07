using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.Turnos.Commands;
using SAsistencia.Application.Features.Turnos.DTOs;
using SAsistencia.Application.Features.Turnos.Queries;

namespace SAsistencia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnosController : ControllerBase
{
    private readonly ISender _mediator;

    public TurnosController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<TurnoDto>>> Listar()
    {
        return Ok(await _mediator.Send(new GetTurnosQuery()));
    }

    [HttpPost]
    public async Task<ActionResult> Crear([FromBody] CrearTurnoRequest request)
    {
        int id = await _mediator.Send(new CrearTurnoCommand(request));
        return Ok(new { id, mensaje = "Turno registrado correctamente." });
    }

    [HttpPut]
    public async Task<ActionResult> Actualizar([FromBody] ActualizarTurnoRequest request)
    {
        var ok = await _mediator.Send(new ActualizarTurnoCommand(request));
        if (!ok) return NotFound();
        return Ok(new { mensaje = "Turno actualizado correctamente." });
    }
}
