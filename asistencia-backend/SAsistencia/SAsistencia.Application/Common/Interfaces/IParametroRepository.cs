using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Interfaces
{
    public interface IParametroRepository
    {
        Task<List<ParametroGlobal>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
        Task<ParametroGlobal?> ObtenerPorClaveAsync(string clave, CancellationToken cancellationToken = default);
        Task<T> ObtenerValorAsync<T>(string clave, T valorPorDefecto, CancellationToken cancellationToken = default);
        Task ActualizarAsync(ParametroGlobal parametro, CancellationToken cancellationToken = default);
    }
}
