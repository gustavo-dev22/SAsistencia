using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Common.Providers;
using SAsistencia.Application.Common.Services;
using SAsistencia.Infrastructure.Persistence;
using SAsistencia.Infrastructure.Persistence.Repositories;
using SAsistencia.Infrastructure.Repositories;
using SAsistencia.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddHttpClient<ISasiAuthService, SasiAuthService>(client =>
            {
                var baseUrl = configuration["SasiSettings:BaseUrl"] ?? "https://localhost:44337/SASI/api/";
                client.BaseAddress = new Uri(baseUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
            });

            services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOrganizacionRepository, OrganizacionRepository>();
            services.AddScoped<ITurnoRepository, TurnoRepository>();
            services.AddScoped<IMarcacionRepository, MarcacionRepository>();
            services.AddScoped<IJustificacionRepository, JustificacionRepository>();
            services.AddScoped<IParametroRepository, ParametroRepository>();
            services.AddScoped<IAuditoriaMarcacionRepository, AuditoriaMarcacionRepository>();
            services.AddScoped<IFeriadoRepository, FeriadoRepository>();

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
