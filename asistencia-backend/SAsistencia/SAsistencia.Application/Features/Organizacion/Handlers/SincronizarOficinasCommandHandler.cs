using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Common.Services;
using SAsistencia.Application.Features.Organizacion.Commands;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Organizacion.Handlers
{
    public class SincronizarOficinasCommandHandler : IRequestHandler<SincronizarOficinasCommand, int>
    {
        private readonly IOrganizacionRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISasiAuthService _sasiService;

        public SincronizarOficinasCommandHandler(IOrganizacionRepository repo, IUnitOfWork unitOfWork, ISasiAuthService sasiService)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _sasiService = sasiService;
        }

        public async Task<int> Handle(SincronizarOficinasCommand request, CancellationToken cancellationToken)
        {
            var datosSasi = await _sasiService.ObtenerOficinasActivasAsync(request.Token, cancellationToken);

            // Guardamos primero los padres para no violar la Foreign Key autorreferenciada
            var entidades = datosSasi.Select(s => new Oficina
            {
                Id = s.IdOficina,
                Nombre = s.Nombre,
                Sigla = s.Sigla,
                OficinaPadreId = s.OficinaPadreId,
                Activo = s.Activo,
                FechaSincronizacion = DateTime.UtcNow
            }).ToList();

            await _repo.SincronizarOficinasAsync(entidades, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entidades.Count;
        }
    }
}
