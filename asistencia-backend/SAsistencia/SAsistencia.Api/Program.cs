using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SAsistencia.Api.Hubs;
using SAsistencia.Api.Services;
using SAsistencia.Application;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Application.Common.Services;
using SAsistencia.Infrastructure;
using SAsistencia.Infrastructure.Services;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Evitar mapeo automático de claims de ASP.NET
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// 2. Registro de HttpContext y Capas de Arquitectura
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IMarcacionNotifier, MarcacionNotifier>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSignalR();

// 3. Configuración del Esquema de Autenticación JWT Bearer
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("Falta configurar JwtSettings:Secret en appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = !string.IsNullOrEmpty(jwtSettings["Issuer"]),
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = !string.IsNullOrEmpty(jwtSettings["Audience"]),
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Soporte para autenticación en WebSockets (SignalR)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/marcaciones"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// 4. Controladores y CORS
builder.Services.AddControllers();
builder.Services.AddOpenApi();

const string corsPolicy = "AllowAngularApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4205", "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "API Control de Asistencias";
        options.Theme = ScalarTheme.Purple;
    });
}

app.UseHttpsRedirection();
app.UseCors(corsPolicy);

// ORDEN VITAL: Authentication ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MarcacionesHub>("/hubs/marcaciones");

app.Run();
