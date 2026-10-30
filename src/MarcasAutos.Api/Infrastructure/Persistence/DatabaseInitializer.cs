using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MarcasAutos.Api.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>Aplica las migraciones pendientes (y el seed asociado) al iniciar la aplicación.</summary>
    public bool ApplyMigrationsOnStartup { get; set; } = true;
}

/// <summary>
/// Aplica las migraciones al arrancar. Se implementa como IHostedService (y no en Program.cs) para que
/// se ejecute antes de que Kestrel acepte tráfico y para que las herramientas de diseño de EF
/// (dotnet ef) puedan construir el host sin tocar la base de datos.
/// El seed se dispara desde MigrateAsync mediante UseAsyncSeeding (ver DependencyInjection).
/// </summary>
public sealed partial class DatabaseInitializer(
    IServiceScopeFactory scopeFactory,
    IOptions<DatabaseOptions> options,
    ILogger<DatabaseInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.ApplyMigrationsOnStartup)
        {
            LogMigracionesDeshabilitadas();
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        LogAplicandoMigraciones();
        await dbContext.Database.MigrateAsync(cancellationToken);
        LogBaseDeDatosLista();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    [LoggerMessage(Level = LogLevel.Information, Message = "Aplicación de migraciones al inicio deshabilitada.")]
    private partial void LogMigracionesDeshabilitadas();

    [LoggerMessage(Level = LogLevel.Information, Message = "Aplicando migraciones de base de datos...")]
    private partial void LogAplicandoMigraciones();

    [LoggerMessage(Level = LogLevel.Information, Message = "Base de datos lista.")]
    private partial void LogBaseDeDatosLista();
}
