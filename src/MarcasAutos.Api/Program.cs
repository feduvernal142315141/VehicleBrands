using MarcasAutos.Api.Application.Services;
using MarcasAutos.Api.Infrastructure;
using MarcasAutos.Api.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerDocs();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IMarcasAutosService, MarcasAutosService>();

var app = builder.Build();

// Respuesta 500 genérica (ProblemDetails) sin exponer detalles internos; la excepción queda en los logs.
app.UseExceptionHandler();
app.UseSwaggerDocs();

// Liveness sin comprobaciones de base de datos: las migraciones se aplican antes de que Kestrel acepte tráfico,
// así que una respuesta 200 implica que la aplicación ya está lista, sin consultar Neon en cada sondeo.
app.MapHealthChecks("/healthz");
app.MapControllers();

app.Run();

// Necesario para que WebApplicationFactory<Program> pueda referenciar el punto de entrada.
public partial class Program;
