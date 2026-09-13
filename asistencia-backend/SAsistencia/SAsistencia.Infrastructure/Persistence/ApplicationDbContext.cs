using Microsoft.EntityFrameworkCore;
using SAsistencia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SAsistencia.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Empleado> Empleados => Set<Empleado>();
        public DbSet<Turno> Turnos => Set<Turno>();
        public DbSet<Oficina> Oficinas => Set<Oficina>();
        public DbSet<Cargo> Cargos => Set<Cargo>();
        public DbSet<Marcacion> Marcaciones => Set<Marcacion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica automáticamente todas las configuraciones Fluent API 
            // que creemos en la carpeta Persistence/Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
