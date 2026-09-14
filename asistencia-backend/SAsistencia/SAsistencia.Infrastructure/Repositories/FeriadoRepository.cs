using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Domain.Entities;
using SAsistencia.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Infrastructure.Repositories
{
    public class FeriadoRepository : IFeriadoRepository
    {
        private readonly ApplicationDbContext _context;

        public FeriadoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FeriadoCalendario>> ObtenerPorAnioAsync(int anio, CancellationToken cancellationToken = default)
        {
            return await _context.FeriadosCalendario
                .Where(f => f.Anio == anio)
                .OrderBy(f => f.Fecha)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<FeriadoCalendario?> ObtenerPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default)
        {
            var fDate = fecha.Date;
            return await _context.FeriadosCalendario
                .FirstOrDefaultAsync(f => f.Fecha == fDate && f.Activo, cancellationToken);
        }

        public async Task<FeriadoCalendario?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.FeriadosCalendario.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task AgregarAsync(FeriadoCalendario feriado, CancellationToken cancellationToken = default)
        {
            await _context.FeriadosCalendario.AddAsync(feriado, cancellationToken);
        }

        public Task ActualizarAsync(FeriadoCalendario feriado, CancellationToken cancellationToken = default)
        {
            _context.FeriadosCalendario.Update(feriado);
            return Task.CompletedTask;
        }

        public Task EliminarAsync(FeriadoCalendario feriado, CancellationToken cancellationToken = default)
        {
            _context.FeriadosCalendario.Remove(feriado);
            return Task.CompletedTask;
        }

        public async Task<bool> EsDiaNoLaboralAsync(DateTime fecha, CancellationToken cancellationToken = default)
        {
            var fDate = fecha.Date;
            return await _context.FeriadosCalendario
                .AnyAsync(f => f.Fecha == fDate && f.Activo, cancellationToken);
        }
    }
}
