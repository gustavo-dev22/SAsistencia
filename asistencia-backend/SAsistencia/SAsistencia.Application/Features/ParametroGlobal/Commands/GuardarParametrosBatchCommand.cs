using MediatR;
using SAsistencia.Application.Features.ParametroGlobal.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.ParametroGlobal.Commands
{
    public record GuardarParametrosBatchCommand(GuardarParametrosBatchRequest Dto, string Usuario) : IRequest<bool>;
}
