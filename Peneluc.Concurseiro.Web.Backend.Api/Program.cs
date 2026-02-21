using Peneluc.Concurseiro.Web.Backend.Api.HealthChecks;
using Peneluc.Concurseiro.Web.Backend.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Cache
builder.Services.AddMemoryCache();

// Camadas da Clean Architecture
builder.Services.AddInfrastructure();

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("Database");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .Enrich.FromLogContext()
                 .WriteTo.Console());
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowWeb");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Middleware de log de requisições do Serilog
app.UseSerilogRequestLogging();

app.MapControllers();
app.MapHealthChecks("/health");


app.Run();

public partial class Program { }

namespace Peneluc.Concurseiro.Web.Backend.Api
{
    public class ApiEntryPoint { }
}
