using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.AuditoriaMarcaciones.Commands;
using SAsistencia.Application.Features.AuditoriaMarcaciones.DTOs;
using SAsistencia.Application.Features.AuditoriaMarcaciones.Queries;
using SAsistencia.Application.Features.Marcaciones.DTOs;

namespace SAsistencia.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuditoriaMarcacionesController : ControllerBase
{
    private readonly ISender _mediator;

    public AuditoriaMarcacionesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("historial")]
    public async Task<ActionResult<PaginatedResult<ItemAuditoriaMarcacionDto>>> Consultar([FromBody] FiltroAuditoriaRequest filtro)
    {
        return Ok(await _mediator.Send(new GetAuditoriaMarcacionesQuery(filtro)));
    }

    [HttpPost("regularizar")]
    public async Task<ActionResult> RegularizarMarca([FromBody] RegistrarMarcaManualCommand command)
    {
        var ok = await _mediator.Send(command);
        if (!ok) return BadRequest(new { mensaje = "No se pudo registrar la regularización manual." });
        return Ok(new { mensaje = "Marcación regularizada y asentada en la bitácora de auditoría." });
    }
}