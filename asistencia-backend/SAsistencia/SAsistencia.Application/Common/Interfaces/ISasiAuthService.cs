using SAsistencia.Application.Features.Auth.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface ISasiAuthService
    {
        Task<LoginResponse> AutenticarAsync(string usuario, string password, CancellationToken cancellationToken = default);
    }
}
