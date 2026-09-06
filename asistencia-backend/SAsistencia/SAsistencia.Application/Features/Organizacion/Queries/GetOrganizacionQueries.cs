using MediatR;
using SAsistencia.Application.Features.Organizacion.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Organizacion.Queries
{
    public record GetOficinasQuery() : IRequest<List<OficinaListDto>>;
    public record GetCargosQuery() : IRequest<List<CargoDto>>;
}
