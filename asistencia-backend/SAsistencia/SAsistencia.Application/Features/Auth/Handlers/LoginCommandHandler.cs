using MediatR;
using SAsistencia.Application.Common.Services;
using SAsistencia.Application.Features.Auth.Commands;
using SAsistencia.Application.Features.Auth.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace SAsistencia.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly ISasiAuthService _sasiAuthService;

        public LoginCommandHandler(ISasiAuthService sasiAuthService)
        {
            _sasiAuthService = sasiAuthService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _sasiAuthService.AutenticarAsync(request.Usuario, request.Password, cancellationToken);
        }
    }
}
