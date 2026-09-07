using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Domain.Entities;
using SAsistencia.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Infrastructure.Repositories
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly ApplicationDbContext _context;

        public TurnoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Turno>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Turnos
                .Include(t => t.Empleados)
                .OrderBy(t => t.HoraEntrada)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Turno?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Turnos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task AgregarAsync(Turno turno, CancellationToken cancellationToken = default)
        {
            await _context.Turnos.AddAsync(turno, cancellationToken);
        }

        public Task ActualizarAsync(Turno turno, CancellationToken cancellationToken = default)
        {
            _context.Turnos.Update(turno);
            return Task.CompletedTask;
        }
    }
}
