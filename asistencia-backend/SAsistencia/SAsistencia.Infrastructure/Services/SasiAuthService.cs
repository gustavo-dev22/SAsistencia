using Microsoft.Extensions.Configuration;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Auth.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace SAsistencia.Infrastructure.Services
{
    public class SasiAuthService : ISasiAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const int ID_SISTEMA_ASISTENCIA = 21;

        public SasiAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
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
    }
}
