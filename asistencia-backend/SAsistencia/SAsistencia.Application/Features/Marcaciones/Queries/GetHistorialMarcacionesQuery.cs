using MediatR;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Marcaciones.Queries
{
    public record GetHistorialMarcacionesQuery(FiltroHistorialMarcacionesRequest Filtro): IRequest<PaginatedResult<ItemHistorialMarcacionDto>>;
}
