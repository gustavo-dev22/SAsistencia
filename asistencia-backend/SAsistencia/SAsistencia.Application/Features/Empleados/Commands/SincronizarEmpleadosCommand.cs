using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Empleados.Commands
{
    public record SincronizarEmpleadosCommand(string? Token = null) : IRequest<int>;
}
