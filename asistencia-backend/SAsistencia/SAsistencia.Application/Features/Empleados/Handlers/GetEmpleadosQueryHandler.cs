using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Empleados.DTOs;
using SAsistencia.Application.Features.Empleados.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Empleados.Handlers
{
    public class GetEmpleadosQueryHandler : IRequestHandler<GetEmpleadosQuery, List<EmpleadoListDto>>
    {
        private readonly IEmpleadoRepository _empleadoRepository;

        public GetEmpleadosQueryHandler(IEmpleadoRepository empleadoRepository)
        {
            _empleadoRepository = empleadoRepository;
        }

        public async Task<List<EmpleadoListDto>> Handle(GetEmpleadosQuery request, CancellationToken cancellationToken)
        {
            var empleados = await _empleadoRepository.ObtenerTodosAsync(cancellationToken);

            return empleados.Select(e => new EmpleadoListDto
            {
                Id = e.Id,
                UsuarioIdSasi = e.UsuarioIdSasi,
                NombreCompleto = e.NombreCompleto,
                Email = e.Email,
                Dni = e.Dni,
                CodigoQr = e.CodigoQr,
                TurnoId = e.TurnoId,
                CargoId = e.CargoId,
                CargoNombre = e.Cargo != null ? e.Cargo.Nombre : "Sin cargo asignado",
                OficinaId = e.OficinaId,
                OficinaNombre = e.Oficina != null ? $"{e.Oficina.Sigla} - {e.Oficina.Nombre}" : "Sin asignar",
                TurnoNombre = e.Turno != null ? e.Turno.Nombre : "Sin asignar",
                HabilitadoParaMarcar = e.HabilitadoParaMarcar
            }).ToList();
        }
    }
}
