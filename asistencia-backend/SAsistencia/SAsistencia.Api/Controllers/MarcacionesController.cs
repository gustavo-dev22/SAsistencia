using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using SAsistencia.Application.Features.Marcaciones.Commands;

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
}
