using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using SAsistencia.Application.Features.Marcaciones.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Marcaciones.Handlers
{
    public class GetHistorialMarcacionesQueryHandler : IRequestHandler<GetHistorialMarcacionesQuery, PaginatedResult<ItemHistorialMarcacionDto>>
    {
        private readonly IMarcacionRepository _marcacionRepo;

        public GetHistorialMarcacionesQueryHandler(IMarcacionRepository marcacionRepo)
        {
            _marcacionRepo = marcacionRepo;
        }

        public async Task<PaginatedResult<ItemHistorialMarcacionDto>> Handle(
            GetHistorialMarcacionesQuery request,
            CancellationToken cancellationToken)
        {
            var filtro = request.Filtro;
            var (items, total) = await _marcacionRepo.ConsultarHistorialPaginadoAsync(filtro, cancellationToken);

            var dtoList = items.Select(m => new ItemHistorialMarcacionDto
            {
                Id = m.Id,
                EmpleadoId = m.EmpleadoId,
                NombreCompleto = m.Empleado?.NombreCompleto ?? "Desconocido",
                Dni = m.Empleado?.Dni,
                OficinaNombre = m.Empleado?.Oficina?.Nombre,
                OficinaSigla = m.Empleado?.Oficina?.Sigla,
                CargoNombre = m.Empleado?.Cargo?.Nombre,
                TurnoNombre = m.Turno?.Nombre ?? "Sin Turno",
                FechaHora = m.FechaHoraMarcacion,
                Fecha = m.FechaHoraMarcacion.ToString("dd/MM/yyyy"),
                Hora = m.FechaHoraMarcacion.ToString("HH:mm:ss"),
                TipoMarcacion = m.TipoMarcacion,
                EstadoPuntualidad = m.EstadoPuntualidad,
                MinutosTardanza = m.MinutosTardanza,
                MetodoRegistro = m.MetodoRegistro,
                IpTerminal = m.IpTerminal
            }).ToList();

            var totalPaginas = (int)Math.Ceiling(total / (double)filtro.RegistrosPorPagina);

            return new PaginatedResult<ItemHistorialMarcacionDto>
            {
                Items = dtoList,
                TotalRegistros = total,
                PaginaActual = filtro.Pagina,
                TotalPaginas = totalPaginas
            };
        }
    }
}
