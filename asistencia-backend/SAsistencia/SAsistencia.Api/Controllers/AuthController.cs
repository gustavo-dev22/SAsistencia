using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAsistencia.Application.Features.Auth.Commands;
using SAsistencia.Application.Features.Auth.DTOs;

namespace SAsistencia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _mediator.Send(new LoginCommand(request.Usuario, request.Password));
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno de autenticación: " + ex.Message });
        }
    }
}
