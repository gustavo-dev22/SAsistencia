using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface IFeriadoRepository
    {
        Task<List<FeriadoCalendario>> ObtenerPorAnioAsync(int anio, CancellationToken cancellationToken = default);
        Task<FeriadoCalendario?> ObtenerPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default);
        Task<FeriadoCalendario?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task AgregarAsync(FeriadoCalendario feriado, CancellationToken cancellationToken = default);
        Task ActualizarAsync(FeriadoCalendario feriado, CancellationToken cancellationToken = default);
        Task EliminarAsync(FeriadoCalendario feriado, CancellationToken cancellationToken = default);
        Task<bool> EsDiaNoLaboralAsync(DateTime fecha, CancellationToken cancellationToken = default);
    }
}
