using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Organizacion.Commands
{
    public record SincronizarOficinasCommand(string? Token = null) : IRequest<int>;
}
