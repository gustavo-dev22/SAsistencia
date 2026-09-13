using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using SAsistencia.Application.Features.Marcaciones.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Marcaciones.Handlers
{
    public class GetMarcacionesHoyQueryHandler : IRequestHandler<GetMarcacionesHoyQuery, ResumenMarcacionesHoyDto>
    {
        private readonly IMarcacionRepository _marcacionRepo;

        public GetMarcacionesHoyQueryHandler(IMarcacionRepository marcacionRepo)
        {
            _marcacionRepo = marcacionRepo;
        }

        public async Task<ResumenMarcacionesHoyDto> Handle(GetMarcacionesHoyQuery request, CancellationToken cancellationToken)
        {
            var hoy = DateTime.Today;

            // Consulta a través de la abstracción del repositorio
            var marcas = await _marcacionRepo.ObtenerMarcacionesDelDiaConRelacionesAsync(hoy, 100, cancellationToken);

            var lista = marcas.Select(m => new MarcacionEnVivoDto
            {
                Id = m.Id,
                EmpleadoId = m.EmpleadoId,
                NombreCompleto = m.Empleado?.NombreCompleto ?? "Desconocido",
                Dni = m.Empleado?.Dni,
                OficinaNombre = m.Empleado?.Oficina?.Nombre,
                OficinaSigla = m.Empleado?.Oficina?.Sigla,
                CargoNombre = m.Empleado?.Cargo?.Nombre,
                Hora = m.FechaHoraMarcacion.ToString("HH:mm:ss"),
                TipoMarcacion = m.TipoMarcacion,
                EstadoPuntualidad = m.EstadoPuntualidad,
                MinutosTardanza = m.MinutosTardanza,
                MetodoRegistro = m.MetodoRegistro,
                FechaHora = m.FechaHoraMarcacion
            }).ToList();

            return new ResumenMarcacionesHoyDto
            {
                TotalMarcas = lista.Count,
                TotalPuntuales = lista.Count(x => x.EstadoPuntualidad == "PUNTUAL" || x.EstadoPuntualidad == "TOLERANCIA"),
                TotalTardanzas = lista.Count(x => x.EstadoPuntualidad == "TARDANZA"),
                TotalExonerados = lista.Count(x => x.EstadoPuntualidad == "EXONERADO"),
                UltimasMarcaciones = lista
            };
        }
    }
}
