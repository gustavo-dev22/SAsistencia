using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Common.Providers;
using SAsistencia.Application.Common.Services;
using SAsistencia.Application.Features.AuditoriaMarcaciones.Commands;
using SAsistencia.Application.Features.AuditoriaMarcaciones.DTOs;
using SAsistencia.Application.Features.AuditoriaMarcaciones.Queries;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.AuditoriaMarcaciones.Handlers
{
    public class AuditoriaHandlers :
    IRequestHandler<GetAuditoriaMarcacionesQuery, PaginatedResult<ItemAuditoriaMarcacionDto>>,
    IRequestHandler<RegistrarMarcaManualCommand, bool>
    {
        private readonly IAuditoriaMarcacionRepository _auditoriaRepo;
        private readonly IMarcacionRepository _marcacionRepo;
        private readonly IEmpleadoRepository _empleadoRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuditoriaHandlers(
            IAuditoriaMarcacionRepository auditoriaRepo,
            IMarcacionRepository marcacionRepo,
            IEmpleadoRepository empleadoRepo,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IDateTimeProvider dateTimeProvider)
        {
            _auditoriaRepo = auditoriaRepo;
            _marcacionRepo = marcacionRepo;
            _empleadoRepo = empleadoRepo;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<PaginatedResult<ItemAuditoriaMarcacionDto>> Handle(
            GetAuditoriaMarcacionesQuery request,
            CancellationToken cancellationToken)
        {
            var (items, total) = await _auditoriaRepo.ConsultarPaginadoAsync(request.Filtro, cancellationToken);

            var dtoList = items.Select(a => new ItemAuditoriaMarcacionDto
            {
                Id = a.Id,
                MarcacionId = a.MarcacionId,
                EmpleadoId = a.EmpleadoId,
                NombreEmpleado = a.Empleado?.NombreCompleto ?? "Desconocido",
                DniEmpleado = a.Empleado?.Dni,
                OficinaSigla = a.Empleado?.Oficina?.Sigla,
                TipoOperacion = a.TipoOperacion,
                HoraAnterior = a.HoraAnterior,
                HoraNueva = a.HoraNueva,
                TipoMarcacion = a.TipoMarcacion,
                MotivoJustificacion = a.MotivoJustificacion,
                UsuarioResponsable = a.UsuarioResponsable,
                IpResponsable = a.IpResponsable,
                FechaRegistro = a.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss")
            }).ToList();

            return new PaginatedResult<ItemAuditoriaMarcacionDto>
            {
                Items = dtoList,
                TotalRegistros = total,
                PaginaActual = request.Filtro.Pagina,
                TotalPaginas = (int)Math.Ceiling(total / (double)request.Filtro.RegistrosPorPagina)
            };
        }

        public async Task<bool> Handle(RegistrarMarcaManualCommand request, CancellationToken cancellationToken)
        {
            var empleado = await _empleadoRepo.ObtenerPorIdAsync(request.EmpleadoId, cancellationToken);
            if (empleado == null) return false;

            var fechaHora = request.Fecha.Date.Add(request.Hora);
            var ahoraPeru = _dateTimeProvider.AhoraPeru;

            // 1. Insertar la Marcación Regularizada
            var nuevaMarca = new Marcacion
            {
                EmpleadoId = empleado.Id,
                TurnoId = empleado.TurnoId,
                FechaHoraMarcacion = fechaHora,
                TipoMarcacion = request.TipoMarcacion,
                EstadoPuntualidad = "JUSTIFICADO",
                MinutosTardanza = 0,
                MetodoRegistro = "MANUAL_RRHH",
                IpTerminal = _currentUserService.ObtenerIpCliente(),
                EsManual = true
            };

            await _marcacionRepo.AgregarAsync(nuevaMarca, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 2. Registrar el Asiento de Auditoría Inmutable
            var bitacora = new AuditoriaMarcacion
            {
                MarcacionId = nuevaMarca.Id,
                EmpleadoId = empleado.Id,
                TipoOperacion = "CREACION_MANUAL",
                HoraAnterior = null,
                HoraNueva = fechaHora.ToString("dd/MM/yyyy HH:mm:ss"),
                TipoMarcacion = request.TipoMarcacion,
                MotivoJustificacion = request.Motivo.Trim(),
                UsuarioResponsable = _currentUserService.NombreCompleto,
                IpResponsable = _currentUserService.ObtenerIpCliente(),
                FechaRegistro = ahoraPeru
            };

            await _auditoriaRepo.AgregarAsync(bitacora, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
