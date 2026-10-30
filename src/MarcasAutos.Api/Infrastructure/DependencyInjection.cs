using MarcasAutos.Api.Infrastructure.Persistence;
using MarcasAutos.Api.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutos.Api.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            // Se lee al resolver el contexto: falla en el arranque (migraciones) si falta la configuración.
            var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException(
                    $"Falta la cadena de conexión 'ConnectionStrings:{ConnectionStringName}'.");

            options
                .UseNpgsql(connectionString)
                // Mecanismo oficial de seeding de EF Core: se ejecuta al final de Migrate/MigrateAsync.
                .UseSeeding((context, _) => MarcasAutosSeeder.Seed(context))
                .UseAsyncSeeding((context, _, cancellationToken) => MarcasAutosSeeder.SeedAsync(context, cancellationToken));
        });

        services.AddHostedService<DatabaseInitializer>();

        return services;
    }
}
