using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Organizacion.DTOs;
using SAsistencia.Application.Features.Organizacion.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Organizacion.Handlers
{
    public class GetOrganizacionQueriesHandler :
    IRequestHandler<GetOficinasQuery, List<OficinaListDto>>,
    IRequestHandler<GetCargosQuery, List<CargoDto>>
    {
        private readonly IOrganizacionRepository _repo;

        public GetOrganizacionQueriesHandler(IOrganizacionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<OficinaListDto>> Handle(GetOficinasQuery request, CancellationToken cancellationToken)
        {
            var oficinas = await _repo.ObtenerOficinasAsync(cancellationToken);
            return oficinas.Select(o => new OficinaListDto
            {
                Id = o.Id,
                Nombre = o.Nombre,
                Sigla = o.Sigla,
                OficinaPadreId = o.OficinaPadreId,
                NombreOficinaPadre = o.OficinaPadre != null
                    ? $"{o.OficinaPadre.Sigla} - {o.OficinaPadre.Nombre}"
                    : "Sede Central",
                Activo = o.Activo,
                TotalEmpleados = o.Empleados.Count // <-- Ahora sí contará a los empleados asignados
            }).ToList();
        }

        public async Task<List<CargoDto>> Handle(GetCargosQuery request, CancellationToken cancellationToken)
        {
            var cargos = await _repo.ObtenerCargosAsync(cancellationToken);
            return cargos.Select(c => new CargoDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                ExoneradoMarcacion = c.ExoneradoMarcacion,
                Activo = c.Activo,
                TotalEmpleados = c.Empleados.Count
            }).ToList();
        }
    }
}
