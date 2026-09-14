using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Common.Providers;
using SAsistencia.Application.Common.Services;
using SAsistencia.Application.Features.CalendarioLaboral.Commands;
using SAsistencia.Application.Features.CalendarioLaboral.DTOs;
using SAsistencia.Application.Features.CalendarioLaboral.Queries;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SAsistencia.Application.Features.CalendarioLaboral.Handlers
{
    public class CalendarioHandlers :
    IRequestHandler<GetFeriadosPorAnioQuery, List<FeriadoItemDto>>,
    IRequestHandler<GuardarFeriadoCommand, bool>,
    IRequestHandler<EliminarFeriadoCommand, bool>
    {
        private readonly IFeriadoRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CalendarioHandlers(
            IFeriadoRepository repo,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IDateTimeProvider dateTimeProvider)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<List<FeriadoItemDto>> Handle(GetFeriadosPorAnioQuery request, CancellationToken cancellationToken)
        {
            var lista = await _repo.ObtenerPorAnioAsync(request.Anio, cancellationToken);
            var culturaPeru = new CultureInfo("es-PE");

            return lista.Select(f => new FeriadoItemDto
            {
                Id = f.Id,
                Anio = f.Anio,
                Fecha = f.Fecha.ToString("yyyy-MM-dd"),
                FechaLegible = f.Fecha.ToString("dd/MM/yyyy"),
                DiaSemana = culturaPeru.TextInfo.ToTitleCase(f.Fecha.ToString("dddd", culturaPeru)),
                Nombre = f.Nombre,
                Tipo = f.Tipo,
                AplicaSectorPublico = f.AplicaSectorPublico,
                EsCompensable = f.EsCompensable,
                NormaLegal = f.NormaLegal,
                FechaCompensacionLimite = f.FechaCompensacionLimite?.ToString("dd/MM/yyyy"),
                Activo = f.Activo
            }).ToList();
        }

        public async Task<bool> Handle(GuardarFeriadoCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var fecha = dto.Fecha.Date;

            if (dto.Id.HasValue && dto.Id.Value > 0)
            {
                var feriado = await _repo.ObtenerPorIdAsync(dto.Id.Value, cancellationToken);
                if (feriado == null) return false;

                feriado.Fecha = fecha;
                feriado.Anio = fecha.Year;
                feriado.Nombre = dto.Nombre.Trim();
                feriado.Tipo = dto.Tipo;
                feriado.AplicaSectorPublico = dto.AplicaSectorPublico;
                feriado.EsCompensable = dto.EsCompensable;
                feriado.NormaLegal = dto.NormaLegal?.Trim();
                feriado.FechaCompensacionLimite = dto.FechaCompensacionLimite?.Date;

                await _repo.ActualizarAsync(feriado, cancellationToken);
            }
            else
            {
                // Evitar duplicar la misma fecha
                var existente = await _repo.ObtenerPorFechaAsync(fecha, cancellationToken);
                if (existente != null) return false;

                var nuevo = new FeriadoCalendario
                {
                    Anio = fecha.Year,
                    Fecha = fecha,
                    Nombre = dto.Nombre.Trim(),
                    Tipo = dto.Tipo,
                    AplicaSectorPublico = dto.AplicaSectorPublico,
                    EsCompensable = dto.EsCompensable,
                    NormaLegal = dto.NormaLegal?.Trim(),
                    FechaCompensacionLimite = dto.FechaCompensacionLimite?.Date,
                    Activo = true,
                    FechaRegistro = _dateTimeProvider.AhoraPeru,
                    UsuarioCreacion = _currentUserService.NombreCompleto
                };

                await _repo.AgregarAsync(nuevo, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(EliminarFeriadoCommand request, CancellationToken cancellationToken)
        {
            var feriado = await _repo.ObtenerPorIdAsync(request.Id, cancellationToken);
            if (feriado == null) return false;

            await _repo.EliminarAsync(feriado, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
