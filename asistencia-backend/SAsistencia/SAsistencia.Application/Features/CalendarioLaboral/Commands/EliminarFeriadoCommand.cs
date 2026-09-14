using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.CalendarioLaboral.Commands
{
    public record EliminarFeriadoCommand(int Id) : IRequest<bool>;
}
