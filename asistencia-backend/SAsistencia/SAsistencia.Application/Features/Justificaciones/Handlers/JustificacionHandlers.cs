using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Justificaciones.Commands;
using SAsistencia.Application.Features.Justificaciones.DTOs;
using SAsistencia.Application.Features.Justificaciones.Queries;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Justificaciones.Handlers
{
    public class JustificacionHandlers :
    IRequestHandler<GetTiposJustificacionQuery, List<TipoJustificacionDto>>,
    IRequestHandler<GetJustificacionesPaginadasQuery, PaginatedResult<JustificacionItemDto>>,
    IRequestHandler<ResolverJustificacionCommand, bool>
    {
        private readonly IJustificacionRepository _justificacionRepo;
        private readonly IMarcacionRepository _marcacionRepo;
        private readonly IUnitOfWork _unitOfWork;

        public JustificacionHandlers(
            IJustificacionRepository justificacionRepo,
            IMarcacionRepository marcacionRepo,
            IUnitOfWork unitOfWork)
        {
            _justificacionRepo = justificacionRepo;
            _marcacionRepo = marcacionRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TipoJustificacionDto>> Handle(GetTiposJustificacionQuery request, CancellationToken cancellationToken)
        {
            var tipos = await _justificacionRepo.ObtenerTiposAsync(cancellationToken);
            return tipos.Select(t => new TipoJustificacionDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                RequiereDocumento = t.RequiereDocumento,
                ConGoceHaber = t.ConGoceHaber
            }).ToList();
        }

        public async Task<PaginatedResult<JustificacionItemDto>> Handle(GetJustificacionesPaginadasQuery request, CancellationToken cancellationToken)
        {
            var (items, total) = await _justificacionRepo.ConsultarPaginadoAsync(request.Filtro, cancellationToken);

            var dtoList = items.Select(j => new JustificacionItemDto
            {
                Id = j.Id,
                EmpleadoId = j.EmpleadoId,
                NombreCompleto = j.Empleado?.NombreCompleto ?? "Desconocido",
                Dni = j.Empleado?.Dni,
                OficinaNombre = j.Empleado?.Oficina?.Nombre,
                OficinaSigla = j.Empleado?.Oficina?.Sigla,
                TipoJustificacionId = j.TipoJustificacionId,
                TipoJustificacionNombre = j.TipoJustificacion?.Nombre ?? "General",
                ConGoceHaber = j.TipoJustificacion?.ConGoceHaber ?? true,
                MarcacionId = j.MarcacionId,
                FechaInicio = j.FechaInicio.ToString("dd/MM/yyyy"),
                FechaFin = j.FechaFin.ToString("dd/MM/yyyy"),
                Motivo = j.Motivo,
                RutaArchivo = j.RutaArchivo,
                NombreArchivoOriginal = j.NombreArchivoOriginal,
                Estado = j.Estado,
                ObservacionesAprobacion = j.ObservacionesAprobacion,
                FechaRegistro = j.FechaRegistro.ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            return new PaginatedResult<JustificacionItemDto>
            {
                Items = dtoList,
                TotalRegistros = total,
                PaginaActual = request.Filtro.Pagina,
                TotalPaginas = (int)Math.Ceiling(total / (double)request.Filtro.RegistrosPorPagina)
            };
        }

        public async Task<bool> Handle(ResolverJustificacionCommand request, CancellationToken cancellationToken)
        {
            var justificacion = await _justificacionRepo.ObtenerPorIdAsync(request.Dto.Id, cancellationToken);
            if (justificacion == null || justificacion.Estado != "PENDIENTE")
                return false;

            justificacion.Estado = request.Dto.Estado;
            justificacion.ObservacionesAprobacion = request.Dto.Observaciones?.Trim();
            justificacion.UsuarioAprobadorId = request.UsuarioAprobador;
            justificacion.FechaAprobacion = DateTime.UtcNow;

            // Efecto colateral: si está asociada a una marcación puntual y es APROBADA, convalidar
            if (request.Dto.Estado == "APROBADA" && justificacion.Marcacion != null)
            {
                justificacion.Marcacion.EstadoPuntualidad = "JUSTIFICADO";
            }

            await _justificacionRepo.ActualizarAsync(justificacion, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
