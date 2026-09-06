using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.Organizacion.Commands;
using SAsistencia.Application.Features.Organizacion.DTOs;
using SAsistencia.Application.Features.Organizacion.Queries;

namespace SAsistencia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizacionController : ControllerBase
{
    private readonly ISender _mediator;

    public OrganizacionController(ISender mediator)
    {
        _mediator = mediator;
    }

    // --- OFICINAS ---
    [HttpGet("oficinas")]
    public async Task<ActionResult<List<OficinaListDto>>> ListarOficinas()
    {
        return Ok(await _mediator.Send(new GetOficinasQuery()));
    }

    [HttpPost("oficinas/sincronizar")]
    public async Task<ActionResult> SincronizarOficinas()
    {
        string? token = null;
        if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var raw = authHeader.ToString();
            token = raw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? raw["Bearer ".Length..].Trim()
                : raw.Trim();
        }

        int total = await _mediator.Send(new SincronizarOficinasCommand(token));
        return Ok(new { mensaje = $"Sincronización exitosa. {total} oficinas actualizadas desde SASI." });
    }

    // --- CARGOS ---
    [HttpGet("cargos")]
    public async Task<ActionResult<List<CargoDto>>> ListarCargos()
    {
        return Ok(await _mediator.Send(new GetCargosQuery()));
    }

    [HttpPost("cargos")]
    public async Task<ActionResult> CrearCargo([FromBody] CrearCargoRequest request)
    {
        int id = await _mediator.Send(new CrearCargoCommand(request));
        return Ok(new { id, mensaje = "Cargo creado exitosamente." });
    }

    [HttpPut("cargos")]
    public async Task<ActionResult> ActualizarCargo([FromBody] ActualizarCargoRequest request)
    {
        var ok = await _mediator.Send(new ActualizarCargoCommand(request));
        if (!ok) return NotFound();
        return Ok(new { mensaje = "Cargo actualizado correctamente." });
    }
}
