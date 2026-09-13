using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.Marcaciones.Commands;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using SAsistencia.Application.Features.Marcaciones.Queries;

namespace SAsistencia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarcacionesController : ControllerBase
{
    private readonly ISender _mediator;

    public MarcacionesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<ResultadoMarcacionDto>> RegistrarMarca([FromBody] RegistrarMarcaRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var resultado = await _mediator.Send(new RegistrarMarcaCommand(request, ip));
        return Ok(resultado);
    }

    [HttpGet("hoy")]
    public async Task<ActionResult<ResumenMarcacionesHoyDto>> ObtenerMarcacionesHoy()
    {
        return Ok(await _mediator.Send(new GetMarcacionesHoyQuery()));
    }

    [HttpPost("historial")]
    public async Task<ActionResult<PaginatedResult<ItemHistorialMarcacionDto>>> ObtenerHistorial([FromBody] FiltroHistorialMarcacionesRequest filtro)
    {
        var resultado = await _mediator.Send(new GetHistorialMarcacionesQuery(filtro));
        return Ok(resultado);
    }
}
