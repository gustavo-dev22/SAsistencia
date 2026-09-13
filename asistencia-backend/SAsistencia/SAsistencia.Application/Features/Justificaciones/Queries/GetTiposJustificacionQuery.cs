using MediatR;
using SAsistencia.Application.Features.Justificaciones.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Justificaciones.Queries
{
    public record GetTiposJustificacionQuery() : IRequest<List<TipoJustificacionDto>>;
}
