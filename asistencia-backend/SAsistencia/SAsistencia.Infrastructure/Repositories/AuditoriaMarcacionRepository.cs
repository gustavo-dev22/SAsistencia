using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.AuditoriaMarcaciones.DTOs;
using SAsistencia.Domain.Entities;
using SAsistencia.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Infrastructure.Repositories
{
    public class AuditoriaMarcacionRepository : IAuditoriaMarcacionRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditoriaMarcacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<AuditoriaMarcacion> Items, int Total)> ConsultarPaginadoAsync(
            FiltroAuditoriaRequest filtro,
            CancellationToken cancellationToken = default)
        {
            var query = _context.AuditoriaMarcaciones
                .Include(a => a.Empleado)
                    .ThenInclude(e => e.Oficina)
                .AsNoTracking()
                .AsQueryable();

            if (filtro.FechaInicio.HasValue)
                query = query.Where(a => a.FechaRegistro >= filtro.FechaInicio.Value.Date);

            if (filtro.FechaFin.HasValue)
                query = query.Where(a => a.FechaRegistro <= filtro.FechaFin.Value.Date.AddDays(1).AddTicks(-1));

            if (!string.IsNullOrWhiteSpace(filtro.TipoOperacion) && filtro.TipoOperacion != "TODOS")
                query = query.Where(a => a.TipoOperacion == filtro.TipoOperacion);

            if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
            {
                var b = filtro.Busqueda.Trim().ToLower();
                query = query.Where(a => a.Empleado.NombreCompleto.ToLower().Contains(b) ||
                                         (a.Empleado.Dni != null && a.Empleado.Dni.Contains(b)) ||
                                         a.UsuarioResponsable.ToLower().Contains(b));
            }

            int total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(a => a.FechaRegistro)
                .Skip((filtro.Pagina - 1) * filtro.RegistrosPorPagina)
                .Take(filtro.RegistrosPorPagina)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task AgregarAsync(AuditoriaMarcacion auditoria, CancellationToken cancellationToken = default)
        {
            await _context.AuditoriaMarcaciones.AddAsync(auditoria, cancellationToken);
        }
    }
}
