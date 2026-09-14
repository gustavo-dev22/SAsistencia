using MediatR;
using SAsistencia.Application.Features.CalendarioLaboral.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.CalendarioLaboral.Commands
{
    public record GuardarFeriadoCommand(GuardarFeriadoRequest Dto) : IRequest<bool>;
}
