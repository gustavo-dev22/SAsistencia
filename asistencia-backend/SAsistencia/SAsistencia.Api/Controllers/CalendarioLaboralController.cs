using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.CalendarioLaboral.Commands;
using SAsistencia.Application.Features.CalendarioLaboral.DTOs;
using SAsistencia.Application.Features.CalendarioLaboral.Queries;

namespace SAsistencia.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarioLaboralController : ControllerBase
{
    private readonly ISender _mediator;

    public CalendarioLaboralController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{anio:int}")]
    public async Task<ActionResult<List<FeriadoItemDto>>> ListarPorAnio(int anio)
    {
        return Ok(await _mediator.Send(new GetFeriadosPorAnioQuery(anio)));
    }

    [HttpPost("guardar")]
    public async Task<ActionResult> Guardar([FromBody] GuardarFeriadoRequest request)
    {
        var ok = await _mediator.Send(new GuardarFeriadoCommand(request));
        if (!ok) return BadRequest(new { mensaje = "No se pudo registrar el feriado o la fecha ya existe en el calendario." });
        return Ok(new { mensaje = "Día del calendario guardado correctamente." });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var ok = await _mediator.Send(new EliminarFeriadoCommand(id));
        if (!ok) return NotFound(new { mensaje = "No se encontró el feriado a eliminar." });
        return Ok(new { mensaje = "Fecha eliminada del calendario oficial." });
    }
}
