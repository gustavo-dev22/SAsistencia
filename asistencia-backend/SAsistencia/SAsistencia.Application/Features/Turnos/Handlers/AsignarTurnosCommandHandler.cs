using System;
using System.Collections.Generic;
using System.Text;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Turnos.Commands;
using MediatR;

namespace SAsistencia.Application.Features.Turnos.Handlers
{
    public class AsignarTurnosCommandHandler :
    IRequestHandler<AsignarTurnoMasivoCommand, int>,
    IRequestHandler<AsignarTurnoPorOficinaCommand, int>
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AsignarTurnosCommandHandler(IEmpleadoRepository empleadoRepository, IUnitOfWork unitOfWork)
        {
            _empleadoRepository = empleadoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(AsignarTurnoMasivoCommand request, CancellationToken cancellationToken)
        {
            if (request.Dto.EmpleadoIds == null || request.Dto.EmpleadoIds.Count == 0)
                return 0;

            await _empleadoRepository.AsignarTurnoMasivoAsync(request.Dto.EmpleadoIds, request.Dto.TurnoId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return request.Dto.EmpleadoIds.Count;
        }

        public async Task<int> Handle(AsignarTurnoPorOficinaCommand request, CancellationToken cancellationToken)
        {
            await _empleadoRepository.AsignarTurnoPorOficinaAsync(request.Dto.OficinaId, request.Dto.TurnoId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return 1;
        }
    }
}
