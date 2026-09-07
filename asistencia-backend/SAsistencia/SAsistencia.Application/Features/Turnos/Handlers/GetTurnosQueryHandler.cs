using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Turnos.Commands;
using SAsistencia.Application.Features.Turnos.DTOs;
using SAsistencia.Application.Features.Turnos.Queries;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Turnos.Handlers
{
    public class TurnoHandlers : IRequestHandler<GetTurnosQuery, List<TurnoDto>>
    {
        private readonly ITurnoRepository _repo;

        public TurnoHandlers(ITurnoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<TurnoDto>> Handle(GetTurnosQuery request, CancellationToken cancellationToken)
        {
            var turnos = await _repo.ObtenerTodosAsync(cancellationToken);
            return turnos.Select(t => new TurnoDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                HoraEntrada = t.HoraEntrada.ToString(@"hh\:mm"),
                HoraSalida = t.HoraSalida.ToString(@"hh\:mm"),
                ToleranciaEntradaMinutos = t.ToleranciaEntradaMinutos,
                LimiteTardanzaMinutos = t.LimiteTardanzaMinutos,
                MinutosRefrigerio = t.MinutosRefrigerio,
                EsRotativo = t.EsRotativo,
                Activo = t.Activo,
                TotalEmpleadosAsignados = t.Empleados.Count
            }).ToList();
        }
    }
}
