using System;
using System.Collections.Generic;
using System.Text;
using SAsistencia.Domain.Entities;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface IMarcacionRepository
    {
        Task<Marcacion?> ObtenerUltimaMarcaHoyAsync(int empleadoId, DateTime fechaHoy, CancellationToken cancellationToken = default);
        Task AgregarAsync(Marcacion marcacion, CancellationToken cancellationToken = default);
        Task<List<Marcacion>> ObtenerMarcacionesHoyAsync(int empleadoId, DateTime fechaHoy, CancellationToken cancellationToken = default);
    }
}
