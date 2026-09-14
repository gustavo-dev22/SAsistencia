using Microsoft.EntityFrameworkCore;
using SAsistencia.Application.Common.Providers;
using SAsistencia.Application.Common.Services;
using SAsistencia.Domain.Entities;
using SAsistencia.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SAsistencia.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService)
            : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        public DbSet<Empleado> Empleados => Set<Empleado>();
        public DbSet<Turno> Turnos => Set<Turno>();
        public DbSet<Oficina> Oficinas => Set<Oficina>();
        public DbSet<Cargo> Cargos => Set<Cargo>();
        public DbSet<Marcacion> Marcaciones => Set<Marcacion>();
        public DbSet<TipoJustificacion> TiposJustificacion => Set<TipoJustificacion>();
        public DbSet<Justificacion> Justificaciones => Set<Justificacion>();
        public DbSet<ParametroGlobal> ParametrosGlobales => Set<ParametroGlobal>();
        public DbSet<AuditoriaMarcacion> AuditoriaMarcaciones => Set<AuditoriaMarcacion>();
        public DbSet<FeriadoCalendario> FeriadosCalendario => Set<FeriadoCalendario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var ahoraPeru = _dateTimeProvider.AhoraPeru;
            var usuarioActual = _currentUserService.NombreCompleto;

            foreach (var entry in ChangeTracker.Entries())
            {
                // Si la entidad tiene FechaCreacion / FechaRegistro y se está insertando
                if (entry.State == EntityState.Added)
                {
                    if (entry.Properties.Any(p => p.Metadata.Name == "FechaCreacion"))
                        entry.Property("FechaCreacion").CurrentValue = ahoraPeru;

                    if (entry.Properties.Any(p => p.Metadata.Name == "FechaRegistro"))
                        entry.Property("FechaRegistro").CurrentValue = ahoraPeru;

                    if (entry.Properties.Any(p => p.Metadata.Name == "UsuarioCreacion") && entry.Property("UsuarioCreacion").CurrentValue == null)
                        entry.Property("UsuarioCreacion").CurrentValue = usuarioActual;
                }

                // Si la entidad tiene FechaModificacion y se está actualizando
                if (entry.State == EntityState.Modified)
                {
                    if (entry.Properties.Any(p => p.Metadata.Name == "FechaModificacion"))
                        entry.Property("FechaModificacion").CurrentValue = ahoraPeru;

                    if (entry.Properties.Any(p => p.Metadata.Name == "UsuarioModificador"))
                        entry.Property("UsuarioModificador").CurrentValue = usuarioActual;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
