using MediatR;
using SAsistencia.Application.Features.Empleados.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Empleados.Commands
{
    public record ActualizarEmpleadoCommand(ActualizarEmpleadoRequest Dto) : IRequest<bool>;
    public record RegenerarQrCommand(int EmpleadoId) : IRequest<string>;
}
