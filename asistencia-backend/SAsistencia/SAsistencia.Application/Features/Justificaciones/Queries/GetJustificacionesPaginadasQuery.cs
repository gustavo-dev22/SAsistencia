using MediatR;
using SAsistencia.Application.Features.Justificaciones.DTOs;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Justificaciones.Queries
{
    public record GetJustificacionesPaginadasQuery(FiltroJustificacionesRequest Filtro) : IRequest<PaginatedResult<JustificacionItemDto>>;
}
