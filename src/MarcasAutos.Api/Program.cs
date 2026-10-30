using MarcasAutos.Api.Application.Services;
using MarcasAutos.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IMarcasAutosService, MarcasAutosService>();

var app = builder.Build();

// Respuesta 500 genérica (ProblemDetails) sin exponer detalles internos; la excepción queda en los logs.
app.UseExceptionHandler();
app.MapControllers();

app.Run();

// Necesario para que WebApplicationFactory<Program> pueda referenciar el punto de entrada.
public partial class Program;
