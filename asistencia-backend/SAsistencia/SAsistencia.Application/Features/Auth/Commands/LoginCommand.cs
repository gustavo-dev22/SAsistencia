using MediatR;
using SAsistencia.Application.Features.Auth.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Auth.Commands
{
    public record LoginCommand(string Usuario, string Password) : IRequest<LoginResponse>;
}
