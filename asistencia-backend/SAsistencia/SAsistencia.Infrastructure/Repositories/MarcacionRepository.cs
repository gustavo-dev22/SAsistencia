using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Interfaces;
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
}
