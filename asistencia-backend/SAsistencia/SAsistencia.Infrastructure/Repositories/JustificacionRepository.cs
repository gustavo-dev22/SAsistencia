using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Justificaciones.DTOs;
using SAsistencia.Domain.Entities;
using SAsistencia.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Infrastructure.Repositories
{
    public class JustificacionRepository : IJustificacionRepository
    {
        private readonly ApplicationDbContext _context;

        public JustificacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipoJustificacion>> ObtenerTiposAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TiposJustificacion
                .Where(t => t.Activo)
                .OrderBy(t => t.Nombre)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Justificacion?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Justificaciones
                .Include(j => j.Empleado)
                .Include(j => j.TipoJustificacion)
                .Include(j => j.Marcacion)
                .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        }

        public async Task<(List<Justificacion> Items, int Total)> ConsultarPaginadoAsync(FiltroJustificacionesRequest filtro, CancellationToken cancellationToken = default)
        {
            var query = _context.Justificaciones
                .Include(j => j.Empleado)
                    .ThenInclude(e => e.Oficina)
                .Include(j => j.TipoJustificacion)
                .Include(j => j.Marcacion)
                .AsNoTracking()
                .AsQueryable();

            if (filtro.FechaInicio.HasValue)
                query = query.Where(j => j.FechaFin >= filtro.FechaInicio.Value.Date);

            if (filtro.FechaFin.HasValue)
                query = query.Where(j => j.FechaInicio <= filtro.FechaFin.Value.Date);

            if (!string.IsNullOrWhiteSpace(filtro.Estado) && filtro.Estado != "TODOS")
                query = query.Where(j => j.Estado == filtro.Estado);

            if (filtro.TipoJustificacionId.HasValue)
                query = query.Where(j => j.TipoJustificacionId == filtro.TipoJustificacionId.Value);

            if (filtro.OficinaId.HasValue)
                query = query.Where(j => j.Empleado.OficinaId == filtro.OficinaId.Value);

            if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
            {
                var b = filtro.Busqueda.Trim().ToLower();
                query = query.Where(j => j.Empleado.NombreCompleto.ToLower().Contains(b) ||
                                         (j.Empleado.Dni != null && j.Empleado.Dni.Contains(b)));
            }

            int total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(j => j.FechaRegistro)
                .Skip((filtro.Pagina - 1) * filtro.RegistrosPorPagina)
                .Take(filtro.RegistrosPorPagina)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task AgregarAsync(Justificacion justificacion, CancellationToken cancellationToken = default)
        {
            await _context.Justificaciones.AddAsync(justificacion, cancellationToken);
        }

        public Task ActualizarAsync(Justificacion justificacion, CancellationToken cancellationToken = default)
        {
            _context.Justificaciones.Update(justificacion);
            return Task.CompletedTask;
        }
    }
}
