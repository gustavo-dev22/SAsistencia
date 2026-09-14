using Microsoft.AspNetCore.Http;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Common.Services;
using System.Security.Claims;

namespace SAsistencia.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;
    private ClaimsPrincipal? User => HttpContext?.User;

    public bool EstaAutenticado => User?.Identity?.IsAuthenticated ?? false;

    public string? UsuarioId
    {
        get
        {
            if (!EstaAutenticado || User == null) return null;

            return BuscarClaim(ClaimTypes.NameIdentifier)
                ?? BuscarClaim("sub")
                ?? BuscarClaim("id")
                ?? BuscarClaim("usuarioId")
                ?? BuscarClaim("uid");
        }
    }

    public string NombreCompleto
    {
        get
        {
            if (HttpContext == null) return "Sistema";
            if (!EstaAutenticado || User == null) return "Anónimo";

            // 1. Matriz de claims que emite SASI para el nombre de la persona
            var nombre = BuscarClaim("nombreCompleto")
                      ?? BuscarClaim("NombreCompleto")
                      ?? BuscarClaim("fullName")
                      ?? BuscarClaim("name")
                      ?? BuscarClaim(ClaimTypes.Name);

            // 2. Si SASI lo envía desglosado en Nombres + Apellidos
            if (string.IsNullOrWhiteSpace(nombre))
            {
                var nombres = BuscarClaim("nombres")
                           ?? BuscarClaim("given_name")
                           ?? BuscarClaim(ClaimTypes.GivenName);

                var apellidos = BuscarClaim("apellidos")
                             ?? BuscarClaim("family_name")
                             ?? BuscarClaim(ClaimTypes.Surname);

                if (!string.IsNullOrWhiteSpace(nombres) || !string.IsNullOrWhiteSpace(apellidos))
                {
                    nombre = $"{nombres} {apellidos}".Trim();
                }
            }

            // 3. Fallbacks con identificadores de cuenta emitidos por SASI
            if (string.IsNullOrWhiteSpace(nombre))
            {
                nombre = BuscarClaim("usuario")
                      ?? BuscarClaim("username")
                      ?? BuscarClaim("unique_name")
                      ?? BuscarClaim(ClaimTypes.Email)
                      ?? BuscarClaim("email")
                      ?? User.Identity?.Name;
            }

            return !string.IsNullOrWhiteSpace(nombre) ? nombre : "Anónimo";
        }
    }

    public string? Email
    {
        get
        {
            if (!EstaAutenticado || User == null) return null;

            return BuscarClaim(ClaimTypes.Email)
                ?? BuscarClaim("email")
                ?? BuscarClaim("correo");
        }
    }

    public string ObtenerIpCliente()
    {
        if (HttpContext == null) return "127.0.0.1";

        var remote = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        // Limpiar IPv6 loopback
        if (remote == "::1" || string.IsNullOrEmpty(remote))
        {
            return "127.0.0.1";
        }

        return remote;
    }

    /// <summary>
    /// Búsqueda insensible a mayúsculas/minúsculas para no fallar
    /// si SASI emite los claims en CamelCase, PascalCase o URI estándar de Identity.
    /// </summary>
    private string? BuscarClaim(string claimType)
    {
        if (User == null) return null;

        return User.Claims
            .FirstOrDefault(c => string.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }
}