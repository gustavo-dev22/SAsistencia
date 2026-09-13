using SAsistencia.Application.Features.Justificaciones.DTOs;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface IJustificacionRepository
    {
        Task<List<TipoJustificacion>> ObtenerTiposAsync(CancellationToken cancellationToken = default);
        Task<Justificacion?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default);
        Task<(List<Justificacion> Items, int Total)> ConsultarPaginadoAsync(FiltroJustificacionesRequest filtro, CancellationToken cancellationToken = default);
        Task AgregarAsync(Justificacion justificacion, CancellationToken cancellationToken = default);
        Task ActualizarAsync(Justificacion justificacion, CancellationToken cancellationToken = default);
    }
}
