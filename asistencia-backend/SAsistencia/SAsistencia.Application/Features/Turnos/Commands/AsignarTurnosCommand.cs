using System;
using System.Collections.Generic;
using System.Text;
using SAsistencia.Application.Features.Turnos.DTOs;
using MediatR;

namespace SAsistencia.Application.Features.Turnos.Commands
{
    // 1. Asignar a empleados específicos seleccionados
    public record AsignarTurnoMasivoCommand(AsignarTurnoMasivoRequest Dto) : IRequest<int>;

    // 2. Asignar a todos los colaboradores de una Oficina
    public record AsignarTurnoPorOficinaCommand(AsignarTurnoPorOficinaRequest Dto) : IRequest<int>;
}
