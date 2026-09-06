using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Domain.Entities;
using SAsistencia.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Infrastructure.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly ApplicationDbContext _context;

        public EmpleadoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Empleado>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Empleados
                .Include(e => e.Oficina) // <-- OBLIGATORIO
                .Include(e => e.Cargo)   // <-- OBLIGATORIO
                .Include(e => e.Turno)
                .OrderBy(e => e.NombreCompleto)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Empleado?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Empleados
                .Include(e => e.Turno)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<List<string>> ObtenerIdsSasiExistentesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Empleados
                .Select(e => e.UsuarioIdSasi)
                .ToListAsync(cancellationToken);
        }

        public async Task AgregarRangoAsync(IEnumerable<Empleado> empleados, CancellationToken cancellationToken = default)
        {
            await _context.Empleados.AddRangeAsync(empleados, cancellationToken);
        }

        public Task ActualizarAsync(Empleado empleado, CancellationToken cancellationToken = default)
        {
            _context.Empleados.Update(empleado);
            return Task.CompletedTask;
        }
    }
}
