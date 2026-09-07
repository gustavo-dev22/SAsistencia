using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface ITurnoRepository
    {
        Task<List<Turno>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
        Task<Turno?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task AgregarAsync(Turno turno, CancellationToken cancellationToken = default);
        Task ActualizarAsync(Turno turno, CancellationToken cancellationToken = default);
    }
}
