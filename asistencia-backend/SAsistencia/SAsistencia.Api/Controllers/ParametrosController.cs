using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.ParametroGlobal.Commands;
using SAsistencia.Application.Features.ParametroGlobal.DTOs;
using SAsistencia.Application.Features.ParametroGlobal.Queries;

namespace SAsistencia.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ParametrosController : ControllerBase
{
    private readonly ISender _mediator;

    public ParametrosController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ParametroGlobalDto>>> Listar()
    {
        return Ok(await _mediator.Send(new GetParametrosGlobalesQuery()));
    }

    [HttpPost("guardar")]
    public async Task<ActionResult> GuardarBatch([FromBody] GuardarParametrosBatchRequest request)
    {
        var ok = await _mediator.Send(new GuardarParametrosBatchCommand(request, string.Empty));
        return Ok(new { mensaje = "Parámetros institucionales guardados correctamente." });
    }

    [HttpGet("diagnostico-auth")]
    public IActionResult DiagnosticoAuth()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        var claims = User.Claims.Select(c => new
        {
            Tipo = c.Type,
            Valor = c.Value
        }).ToList();

        return Ok(new
        {
            TieneHeaderAuthorization = !string.IsNullOrEmpty(authHeader),
            CabeceraAuth = string.IsNullOrEmpty(authHeader) ? "NO PRESENTE" : (authHeader.Length > 20 ? authHeader.Substring(0, 20) + "..." : authHeader),
            EstaAutenticado = isAuthenticated,
            IdentityName = User.Identity?.Name,
            TotalClaims = claims.Count,
            ListaClaims = claims
        });
    }
}