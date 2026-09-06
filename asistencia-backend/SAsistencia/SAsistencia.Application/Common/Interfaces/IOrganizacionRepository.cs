using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface IOrganizacionRepository
    {
        // Oficinas
        Task<List<Oficina>> ObtenerOficinasAsync(CancellationToken cancellationToken = default);
        Task<List<int>> ObtenerIdsOficinasExistentesAsync(CancellationToken cancellationToken = default);
        Task SincronizarOficinasAsync(IEnumerable<Oficina> oficinas, CancellationToken cancellationToken = default);

        // Cargos
        Task<List<Cargo>> ObtenerCargosAsync(CancellationToken cancellationToken = default);
        Task<Cargo?> ObtenerCargoPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task AgregarCargoAsync(Cargo cargo, CancellationToken cancellationToken = default);
        Task ActualizarCargoAsync(Cargo cargo, CancellationToken cancellationToken = default);
    }
}
