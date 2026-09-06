using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Empleados.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Empleados.Handlers
{
    public class ActualizarEmpleadoCommandHandler :
    IRequestHandler<ActualizarEmpleadoCommand, bool>,
    IRequestHandler<RegenerarQrCommand, string>
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarEmpleadoCommandHandler(IEmpleadoRepository empleadoRepository, IUnitOfWork unitOfWork)
        {
            _empleadoRepository = empleadoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ActualizarEmpleadoCommand request, CancellationToken cancellationToken)
        {
            var emp = await _empleadoRepository.ObtenerPorIdAsync(request.Dto.Id, cancellationToken);
            if (emp == null) return false;

            emp.Dni = request.Dto.Dni;
            emp.TurnoId = request.Dto.TurnoId;
            emp.CargoId = request.Dto.CargoId;
            emp.HabilitadoParaMarcar = request.Dto.HabilitadoParaMarcar;

            await _empleadoRepository.ActualizarAsync(emp, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<string> Handle(RegenerarQrCommand request, CancellationToken cancellationToken)
        {
            var emp = await _empleadoRepository.ObtenerPorIdAsync(request.EmpleadoId, cancellationToken);
            if (emp == null) throw new KeyNotFoundException("Empleado no encontrado");

            emp.CodigoQr = Guid.NewGuid().ToString("N")[..12].ToUpper();

            await _empleadoRepository.ActualizarAsync(emp, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return emp.CodigoQr;
        }
    }
}
