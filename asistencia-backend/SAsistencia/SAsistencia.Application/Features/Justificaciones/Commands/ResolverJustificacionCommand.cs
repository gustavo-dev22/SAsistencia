using MediatR;
using SAsistencia.Application.Features.Justificaciones.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Justificaciones.Commands
{
    public record ResolverJustificacionCommand(ResolverJustificacionRequest Dto, string UsuarioAprobador) : IRequest<bool>;
}
