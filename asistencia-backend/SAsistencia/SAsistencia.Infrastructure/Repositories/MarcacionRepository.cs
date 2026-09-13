using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Features.Marcaciones.DTOs;
using SAsistencia.Domain.Entities;

namespace SAsistencia.Infrastructure.Persistence.Repositories;

public class MarcacionRepository : IMarcacionRepository
{
    private readonly ApplicationDbContext _context;

    public MarcacionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Marcacion?> ObtenerUltimaMarcaHoyAsync(int empleadoId, DateTime fechaHoy, CancellationToken cancellationToken = default)
    {
        var inicioDia = fechaHoy.Date;
        var finDia = inicioDia.AddDays(1);

        return await _context.Marcaciones
            .Where(m => m.EmpleadoId == empleadoId && m.FechaHoraMarcacion >= inicioDia && m.FechaHoraMarcacion < finDia)
            .OrderByDescending(m => m.FechaHoraMarcacion)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AgregarAsync(Marcacion marcacion, CancellationToken cancellationToken = default)
    {
        await _context.Marcaciones.AddAsync(marcacion, cancellationToken);
    }

    public async Task<List<Marcacion>> ObtenerMarcacionesHoyAsync(int empleadoId, DateTime fechaHoy, CancellationToken cancellationToken = default)
    {
        var inicioDia = fechaHoy.Date;
        var finDia = inicioDia.AddDays(1);

        return await _context.Marcaciones
            .Where(m => m.EmpleadoId == empleadoId && m.FechaHoraMarcacion >= inicioDia && m.FechaHoraMarcacion < finDia)
            .OrderBy(m => m.FechaHoraMarcacion)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Marcacion>> ObtenerMarcacionesDelDiaConRelacionesAsync(DateTime fecha, int limite = 100, CancellationToken cancellationToken = default)
    {
        var inicioDia = fecha.Date;
        var finDia = inicioDia.AddDays(1);

        return await _context.Marcaciones
            .Include(m => m.Empleado)
                .ThenInclude(e => e.Oficina)
            .Include(m => m.Empleado)
                .ThenInclude(e => e.Cargo)
            .Where(m => m.FechaHoraMarcacion >= inicioDia && m.FechaHoraMarcacion < finDia)
            .OrderByDescending(m => m.FechaHoraMarcacion)
            .Take(limite)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Marcacion> Items, int Total)> ConsultarHistorialPaginadoAsync(FiltroHistorialMarcacionesRequest filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.Marcaciones
            .Include(m => m.Empleado)
                .ThenInclude(e => e.Oficina)
            .Include(m => m.Empleado)
                .ThenInclude(e => e.Cargo)
            .Include(m => m.Turno)
            .AsNoTracking()
            .AsQueryable();

        // 1. Rango de Fechas
        var inicio = filtro.FechaInicio.Date;
        var fin = filtro.FechaFin.Date.AddDays(1).AddTicks(-1);
        query = query.Where(m => m.FechaHoraMarcacion >= inicio && m.FechaHoraMarcacion <= fin);

        // 2. Filtros Específicos
        if (filtro.EmpleadoId.HasValue)
            query = query.Where(m => m.EmpleadoId == filtro.EmpleadoId.Value);

        if (filtro.OficinaId.HasValue)
            query = query.Where(m => m.Empleado.OficinaId == filtro.OficinaId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.TipoMarcacion))
            query = query.Where(m => m.TipoMarcacion == filtro.TipoMarcacion);

        if (!string.IsNullOrWhiteSpace(filtro.EstadoPuntualidad))
            query = query.Where(m => m.EstadoPuntualidad == filtro.EstadoPuntualidad);

        // 3. Búsqueda por DNI o Nombre
        if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
        {
            var b = filtro.Busqueda.Trim().ToLower();
            query = query.Where(m => m.Empleado.NombreCompleto.ToLower().Contains(b) ||
                                     (m.Empleado.Dni != null && m.Empleado.Dni.Contains(b)));
        }

        int total = await query.CountAsync(cancellationToken);

        // 4. Paginación
        var items = await query
            .OrderByDescending(m => m.FechaHoraMarcacion)
            .Skip((filtro.Pagina - 1) * filtro.RegistrosPorPagina)
            .Take(filtro.RegistrosPorPagina)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
