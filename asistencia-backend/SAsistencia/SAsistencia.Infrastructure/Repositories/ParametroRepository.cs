using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Domain.Entities;
using SAsistencia.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SAsistencia.Infrastructure.Repositories
{
    public class ParametroRepository : IParametroRepository
    {
        private readonly ApplicationDbContext _context;

        public ParametroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParametroGlobal>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ParametrosGlobales
                .OrderBy(p => p.Categoria)
                .ThenBy(p => p.Etiqueta)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ParametroGlobal?> ObtenerPorClaveAsync(string clave, CancellationToken cancellationToken = default)
        {
            return await _context.ParametrosGlobales
                .FirstOrDefaultAsync(p => p.Clave == clave, cancellationToken);
        }

        public async Task<T> ObtenerValorAsync<T>(string clave, T valorPorDefecto, CancellationToken cancellationToken = default)
        {
            var param = await _context.ParametrosGlobales
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Clave == clave, cancellationToken);

            if (param == null || string.IsNullOrWhiteSpace(param.Valor))
                return valorPorDefecto;

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                {
                    return (T)converter.ConvertFromString(param.Valor)!;
                }
                return (T)Convert.ChangeType(param.Valor, typeof(T));
            }
            catch
            {
                return valorPorDefecto;
            }
        }

        public Task ActualizarAsync(ParametroGlobal parametro, CancellationToken cancellationToken = default)
        {
            _context.ParametrosGlobales.Update(parametro);
            return Task.CompletedTask;
        }
    }
}
