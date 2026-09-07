using MediatR;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Turnos.Commands;

namespace SAsistencia.Application.Features.Turnos.Handlers
{
    public class ActualizarTurnoCommandHandler : IRequestHandler<ActualizarTurnoCommand, bool>
    {
        private readonly ITurnoRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarTurnoCommandHandler(ITurnoRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ActualizarTurnoCommand request, CancellationToken cancellationToken)
        {
            var turno = await _repo.ObtenerPorIdAsync(request.Dto.Id, cancellationToken);
            if (turno == null) return false;

            turno.Nombre = request.Dto.Nombre.Trim();
            turno.Descripcion = request.Dto.Descripcion?.Trim();
            turno.HoraEntrada = TimeSpan.Parse(request.Dto.HoraEntrada);
            turno.HoraSalida = TimeSpan.Parse(request.Dto.HoraSalida);
            turno.ToleranciaEntradaMinutos = request.Dto.ToleranciaEntradaMinutos;
            turno.LimiteTardanzaMinutos = request.Dto.LimiteTardanzaMinutos;
            turno.MinutosRefrigerio = request.Dto.MinutosRefrigerio;
            turno.EsRotativo = request.Dto.EsRotativo;
            turno.Activo = request.Dto.Activo;

            await _repo.ActualizarAsync(turno, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
