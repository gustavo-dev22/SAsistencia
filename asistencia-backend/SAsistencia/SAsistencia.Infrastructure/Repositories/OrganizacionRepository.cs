using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Domain.Entities;
using SAsistencia.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Infrastructure.Repositories
{
    public class OrganizacionRepository : IOrganizacionRepository
    {
        private readonly ApplicationDbContext _context;

        public OrganizacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Oficina>> ObtenerOficinasAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Oficinas
                .Include(o => o.OficinaPadre)
                .Include(o => o.Empleados)
                .OrderBy(o => o.Nombre)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<int>> ObtenerIdsOficinasExistentesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Oficinas.Select(o => o.Id).ToListAsync(cancellationToken);
        }

        public async Task SincronizarOficinasAsync(IEnumerable<Oficina> oficinas, CancellationToken cancellationToken = default)
        {
            var listaEntrante = oficinas.ToList();
            var idsEntrantes = listaEntrante.Select(o => o.Id).ToList();

            var existentes = await _context.Oficinas
                .Where(o => idsEntrantes.Contains(o.Id))
                .ToListAsync(cancellationToken);

            // 1. Primero insertar o actualizar datos básicos sin las relaciones
            foreach (var ofi in listaEntrante)
            {
                var encontrada = existentes.FirstOrDefault(e => e.Id == ofi.Id);
                if (encontrada == null)
                {
                    _context.Oficinas.Add(new Oficina
                    {
                        Id = ofi.Id,
                        Nombre = ofi.Nombre,
                        Sigla = ofi.Sigla,
                        Activo = ofi.Activo,
                        OficinaPadreId = null, // Se asigna en el segundo pase
                        FechaSincronizacion = DateTime.UtcNow
                    });
                }
                else
                {
                    encontrada.Nombre = ofi.Nombre;
                    encontrada.Sigla = ofi.Sigla;
                    encontrada.Activo = ofi.Activo;
                    encontrada.FechaSincronizacion = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            // 2. Segundo pase: vincular las claves foráneas IdOficinaPadre
            var todasLasOficinas = await _context.Oficinas.ToListAsync(cancellationToken);
            foreach (var ofi in listaEntrante)
            {
                var local = todasLasOficinas.FirstOrDefault(o => o.Id == ofi.Id);
                if (local != null)
                {
                    local.OficinaPadreId = ofi.OficinaPadreId;
                }
            }
        }

        public async Task<List<Cargo>> ObtenerCargosAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Cargos
                .Include(c => c.Empleados)
                .OrderBy(c => c.Nombre)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Cargo?> ObtenerCargoPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Cargos.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task AgregarCargoAsync(Cargo cargo, CancellationToken cancellationToken = default)
        {
            await _context.Cargos.AddAsync(cargo, cancellationToken);
        }

        public Task ActualizarCargoAsync(Cargo cargo, CancellationToken cancellationToken = default)
        {
            _context.Cargos.Update(cargo);
            return Task.CompletedTask;
        }
    }
}
