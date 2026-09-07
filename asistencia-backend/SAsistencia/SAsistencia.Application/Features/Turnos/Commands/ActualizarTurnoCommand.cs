using MediatR;
using SAsistencia.Application.Features.Turnos.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Turnos.Commands
{
    public record ActualizarTurnoCommand(ActualizarTurnoRequest Dto) : IRequest<bool>;
}
