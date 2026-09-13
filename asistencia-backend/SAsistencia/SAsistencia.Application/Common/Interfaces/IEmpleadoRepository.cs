using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<List<Empleado>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
        Task<Empleado?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<string>> ObtenerIdsSasiExistentesAsync(CancellationToken cancellationToken = default);
        Task AgregarRangoAsync(IEnumerable<Empleado> empleados, CancellationToken cancellationToken = default);
        Task ActualizarAsync(Empleado empleado, CancellationToken cancellationToken = default);
        Task AsignarTurnoMasivoAsync(IEnumerable<int> empleadoIds, int? turnoId, CancellationToken cancellationToken = default);
        Task AsignarTurnoPorOficinaAsync(int oficinaId, int? turnoId, CancellationToken cancellationToken = default);
        Task<Empleado?> ObtenerPorIdentificadorConRelacionesAsync(string identificador, CancellationToken cancellationToken = default);
    }
}
