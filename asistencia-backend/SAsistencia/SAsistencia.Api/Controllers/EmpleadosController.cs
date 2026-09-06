using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.Empleados.Commands;
using SAsistencia.Application.Features.Empleados.DTOs;
using SAsistencia.Application.Features.Empleados.Queries;

namespace SAsistencia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly ISender _mediator;

    public EmpleadosController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmpleadoListDto>>> Listar()
    {
        return Ok(await _mediator.Send(new GetEmpleadosQuery()));
    }

    [HttpPost("sincronizar")]
    public async Task<ActionResult> Sincronizar()
    {
        // Extraer el token recibido directamente desde el Controller
        string? token = null;
        if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var raw = authHeader.ToString();
            token = raw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? raw["Bearer ".Length..].Trim()
                : raw.Trim();
        }

        int totalNuevos = await _mediator.Send(new SincronizarEmpleadosCommand(token));
        return Ok(new { mensaje = $"Sincronización completada. {totalNuevos} nuevos empleados registrados." });
    }

    [HttpPut]
    public async Task<ActionResult> Actualizar([FromBody] ActualizarEmpleadoRequest request)
    {
        var success = await _mediator.Send(new ActualizarEmpleadoCommand(request));
        if (!success) return NotFound();
        return Ok(new { mensaje = "Empleado actualizado correctamente." });
    }

    [HttpPost("{id}/regenerar-qr")]
    public async Task<ActionResult> RegenerarQr(int id)
    {
        var nuevoQr = await _mediator.Send(new RegenerarQrCommand(id));
        return Ok(new { codigoQr = nuevoQr, mensaje = "Código QR regenerado exitosamente." });
    }
}