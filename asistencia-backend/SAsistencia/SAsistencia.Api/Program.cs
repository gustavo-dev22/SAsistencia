using SAsistencia.Api.Hubs;
using SAsistencia.Api.Services;
using SAsistencia.Application;
using SAsistencia.Application.Common.Interfaces;
using SAsistencia.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IMarcacionNotifier, MarcacionNotifier>();

builder.Services.AddSignalR();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

const string corsPolicy = "AllowAngularApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4205") // URL local típica de Angular
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
app.UseAuthorization();
app.MapControllers();
app.MapHub<MarcacionesHub>("/hubs/marcaciones");

app.Run();
