using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Turnos.Commands;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Turnos.Handlers
{
    public class CrearTurnoCommandHandler : IRequestHandler<CrearTurnoCommand, int> {
        private readonly ITurnoRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public CrearTurnoCommandHandler(ITurnoRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CrearTurnoCommand request, CancellationToken cancellationToken)
        {
            var turno = new Turno
            {
                Nombre = request.Dto.Nombre.Trim(),
                Descripcion = request.Dto.Descripcion?.Trim(),
                HoraEntrada = TimeSpan.Parse(request.Dto.HoraEntrada),
                HoraSalida = TimeSpan.Parse(request.Dto.HoraSalida),
                ToleranciaEntradaMinutos = request.Dto.ToleranciaEntradaMinutos,
                LimiteTardanzaMinutos = request.Dto.LimiteTardanzaMinutos,
                MinutosRefrigerio = request.Dto.MinutosRefrigerio,
                EsRotativo = request.Dto.EsRotativo,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            await _repo.AgregarAsync(turno, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return turno.Id;
        }
    }
}
