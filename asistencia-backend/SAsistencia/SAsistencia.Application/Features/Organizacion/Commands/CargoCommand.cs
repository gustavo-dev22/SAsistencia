using MediatR;
using SAsistencia.Application.Features.Organizacion.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Organizacion.Commands
{
    public record CrearCargoCommand(CrearCargoRequest Dto) : IRequest<int>;
    public record ActualizarCargoCommand(ActualizarCargoRequest Dto) : IRequest<bool>;
}
