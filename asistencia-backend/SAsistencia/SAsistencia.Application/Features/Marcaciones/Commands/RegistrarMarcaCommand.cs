using System;
using System.Collections.Generic;
using System.Text;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using MediatR;

namespace SAsistencia.Application.Features.Marcaciones.Commands
{
    public record RegistrarMarcaCommand(RegistrarMarcaRequest Dto, string? IpTerminal = null) : IRequest<ResultadoMarcacionDto>;
}
