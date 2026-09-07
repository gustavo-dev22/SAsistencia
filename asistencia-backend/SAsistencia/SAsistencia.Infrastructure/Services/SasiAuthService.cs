using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Auth.DTOs;
using SAsistencia.Application.Features.Empleados.DTOs;
using SAsistencia.Application.Features.Organizacion.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace SAsistencia.Infrastructure.Services
{
    public class SasiAuthService : ISasiAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int ID_SISTEMA_ASISTENCIA = 21;

        public SasiAuthService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponse> AutenticarAsync(string usuario, string password, CancellationToken cancellationToken = default)
        {
            var baseUrl = _configuration["SasiSettings:BaseUrl"] ?? "https://localhost:44337/SASI/api/";
            var endpoint = _configuration["SasiSettings:LoginEndpoint"] ?? "auth/login";
            var requestUri = $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";

            // Cuerpo exacto requerido por SASI
            var requestBody = new
            {
                userName = usuario,
                password = password
            };

            var response = await _httpClient.PostAsJsonAsync(requestUri, requestBody, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas en el servicio central SASI.");
            }

            var sasiData = await response.Content.ReadFromJsonAsync<SasiLoginApiResponse>(cancellationToken: cancellationToken);

            if (sasiData == null || !sasiData.Success || sasiData.Usuario == null)
            {
                throw new UnauthorizedAccessException("Acceso denegado o usuario inactivo en SASI.");
            }

            // Filtrar exclusivamente el Sistema de Control de Asistencias (ID = 21)
            var sistemaAsistencia = sasiData.Usuario.Sistemas
                .FirstOrDefault(s => s.Id == ID_SISTEMA_ASISTENCIA && s.Activo);

            if (sistemaAsistencia == null)
            {
                throw new UnauthorizedAccessException("El usuario no tiene asignado el Sistema de Control de Asistencias.");
            }

            // Obtener rol asignado (priorizar el rol principal)
            var rolActivo = sistemaAsistencia.Roles.FirstOrDefault(r => r.EsPrincipal && r.Activo)
                         ?? sistemaAsistencia.Roles.FirstOrDefault(r => r.Activo);

            if (rolActivo == null)
            {
                throw new UnauthorizedAccessException("El usuario no tiene un rol activo en este sistema.");
            }

            // Filtrar y jerarquizar los menús (Menus principales y sus submenús hijos)
            var objetos = rolActivo.Objetos.Where(o => o.Activo).ToList();

            var menus = objetos
                .Where(o => o.Tipo.Equals("Menu", StringComparison.OrdinalIgnoreCase) && o.IdPadre == null)
                .OrderBy(o => o.Orden)
                .Select(m => new MenuItemDto
                {
                    IdObjeto = m.IdObjeto,
                    Titulo = string.IsNullOrWhiteSpace(m.Titulo) ? m.Nombre : m.Titulo,
                    Nombre = m.Nombre,
                    Url = m.Url,
                    Icono = m.Icono,
                    Orden = m.Orden,
                    Submenus = objetos
                        .Where(s => s.IdPadre == m.IdObjeto)
                        .OrderBy(s => s.Orden)
                        .Select(s => new MenuItemDto
                        {
                            IdObjeto = s.IdObjeto,
                            Titulo = string.IsNullOrWhiteSpace(s.Titulo) ? s.Nombre : s.Titulo,
                            Nombre = s.Nombre,
                            Url = s.Url,
                            Icono = s.Icono,
                            Orden = s.Orden
                        }).ToList()
                }).ToList();

            return new LoginResponse
            {
                Success = true,
                Token = sasiData.Token,
                RefreshToken = sasiData.RefreshToken,
                Expiration = sasiData.Expiration,
                Usuario = new UsuarioInfoDto
                {
                    Id = sasiData.Usuario.Id,
                    NombreCompleto = sasiData.Usuario.NombreCompleto,
                    UserName = sasiData.Usuario.UserName,
                    Email = sasiData.Usuario.Email,
                    Oficina = sasiData.Usuario.Oficina?.Nombre ?? "Sin Asignar"
                },
                RolActivo = new RolAsistenciaDto
                {
                    IdRol = rolActivo.IdRol,
                    NombreRol = rolActivo.NombreRol,
                    EsPrincipal = rolActivo.EsPrincipal
                },
                Menus = menus
            };
        }

        public async Task<List<SasiUsuarioItemDto>> ObtenerUsuariosSistemaAsync(
    int sistemaId = 21,
    string? token = null,
    CancellationToken cancellationToken = default)
        {
            var baseUrl = _configuration["SasiSettings:BaseUrl"] ?? "https://localhost:44337/SASI/api/";
            var requestUri = $"{baseUrl.TrimEnd('/')}/sistemas/{sistemaId}/usuarios";

            // 1. Si no vino el token por parámetro, intentar sacarlo de IHttpContextAccessor
            if (string.IsNullOrWhiteSpace(token))
            {
                var httpContext = _httpContextAccessor?.HttpContext;
                if (httpContext != null && httpContext.Request.Headers.TryGetValue("Authorization", out var headerValue))
                {
                    var raw = headerValue.ToString();
                    token = raw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                        ? raw["Bearer ".Length..].Trim()
                        : raw.Trim();
                }
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("No se proporcionó un token de autorización válido para consultar SASI.");
            }

            // 2. Preparar la petición hacia SASI
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Error al consultar usuarios en SASI ({response.StatusCode}): {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<SasiUsuariosResponse>(cancellationToken: cancellationToken);
            return result?.Datos ?? new List<SasiUsuarioItemDto>();
        }

        public async Task<List<SasiOficinaItemDto>> ObtenerOficinasActivasAsync(string? token = null, CancellationToken cancellationToken = default)
        {
            var baseUrl = _configuration["SasiSettings:BaseUrl"] ?? "https://localhost:44337/SASI/api/";
            var requestUri = $"{baseUrl.TrimEnd('/')}/oficinas/activas";

            if (string.IsNullOrWhiteSpace(token))
            {
                var httpContext = _httpContextAccessor?.HttpContext;
                if (httpContext != null && httpContext.Request.Headers.TryGetValue("Authorization", out var headerValue))
                {
                    var raw = headerValue.ToString();
                    token = raw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                        ? raw["Bearer ".Length..].Trim()
                        : raw.Trim();
                }
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Error al consultar oficinas en SASI ({response.StatusCode}): {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<SasiOficinasResponse>(cancellationToken: cancellationToken);
            return result?.Datos ?? new List<SasiOficinaItemDto>();
        }
    }
}
