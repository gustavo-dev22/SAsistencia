using Microsoft.EntityFrameworkCore;
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

        // Aquí registraremos los DbSet cuando creemos las entidades (ej. Empleados, Asistencias)
        // public DbSet<Empleado> Empleados => Set<Empleado>();
        // public DbSet<Asistencia> Asistencias => Set<Asistencia>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica automáticamente todas las configuraciones Fluent API 
            // que creemos en la carpeta Persistence/Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
