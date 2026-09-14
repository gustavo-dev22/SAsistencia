using MediatR;
using SAsistencia.Application.Features.AuditoriaMarcaciones.DTOs;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.AuditoriaMarcaciones.Queries
{
    public record GetAuditoriaMarcacionesQuery(FiltroAuditoriaRequest Filtro) : IRequest<PaginatedResult<ItemAuditoriaMarcacionDto>>;
}
