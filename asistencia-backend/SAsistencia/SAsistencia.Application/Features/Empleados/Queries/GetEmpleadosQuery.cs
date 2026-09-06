using MediatR;
using SAsistencia.Application.Features.Empleados.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Empleados.Queries
{
    public record GetEmpleadosQuery() : IRequest<List<EmpleadoListDto>>;
}
