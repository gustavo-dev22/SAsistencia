using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Organizacion.Commands;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Organizacion.Handlers
{
    public class CargoCommandHandler :
    IRequestHandler<CrearCargoCommand, int>,
    IRequestHandler<ActualizarCargoCommand, bool>
    {
        private readonly IOrganizacionRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public CargoCommandHandler(IOrganizacionRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CrearCargoCommand request, CancellationToken cancellationToken)
        {
            var cargo = new Cargo
            {
                Nombre = request.Dto.Nombre.Trim(),
                Descripcion = request.Dto.Descripcion?.Trim(),
                ExoneradoMarcacion = request.Dto.ExoneradoMarcacion,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            await _repo.AgregarCargoAsync(cargo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return cargo.Id;
        }

        public async Task<bool> Handle(ActualizarCargoCommand request, CancellationToken cancellationToken)
        {
            var cargo = await _repo.ObtenerCargoPorIdAsync(request.Dto.Id, cancellationToken);
            if (cargo == null) return false;

            cargo.Nombre = request.Dto.Nombre.Trim();
            cargo.Descripcion = request.Dto.Descripcion?.Trim();
            cargo.ExoneradoMarcacion = request.Dto.ExoneradoMarcacion;
            cargo.Activo = request.Dto.Activo;

            await _repo.ActualizarCargoAsync(cargo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
