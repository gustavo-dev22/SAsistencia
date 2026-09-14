using SAsistencia.Application.Features.Auth.DTOs;
using SAsistencia.Application.Features.Empleados.DTOs;
using SAsistencia.Application.Features.Organizacion.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Services
{
    public interface ISasiAuthService
    {
        Task<LoginResponse> AutenticarAsync(string usuario, string password, CancellationToken cancellationToken = default);
        Task<List<SasiUsuarioItemDto>> ObtenerUsuariosSistemaAsync(int sistemaId = 21, string? token = null, CancellationToken cancellationToken = default);
        Task<List<SasiOficinaItemDto>> ObtenerOficinasActivasAsync(string? token = null, CancellationToken cancellationToken = default);
    }
}
