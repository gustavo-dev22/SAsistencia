using MediatR;
using SAsistencia.Application.Features.ParametroGlobal.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.ParametroGlobal.Queries
{
    public record GetParametrosGlobalesQuery() : IRequest<List<ParametroGlobalDto>>;
}
