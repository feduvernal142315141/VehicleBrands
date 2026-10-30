using MarcasAutos.Api.Application.Services;
using MarcasAutos.Api.Domain.Entities;
using MarcasAutos.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MarcasAutos.Tests.Fixtures;

/// <summary>
/// Levanta la API completa en memoria sustituyendo PostgreSQL por InMemory.
/// Cada instancia usa su propia base de datos, por lo que los tests quedan aislados.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"api-{Guid.NewGuid()}";

    public Type? ServiceReemplazado { get; init; }

    /// <summary>Por defecto Production, como en Render; así se comprueba que Swagger no depende de Development.</summary>
    public string Entorno { get; init; } = Environments.Production;

    public bool? SwaggerHabilitado { get; init; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Las migraciones son relacionales y no aplican a InMemory.
        builder.UseSetting("Database:ApplyMigrationsOnStartup", "false");
        builder.UseEnvironment(Entorno);

        if (SwaggerHabilitado is { } habilitado)
        {
            builder.UseSetting("Swagger:Enabled", habilitado.ToString());
        }

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));

            if (ServiceReemplazado is not null)
            {
                services.RemoveAll<IMarcasAutosService>();
                services.AddScoped(typeof(IMarcasAutosService), ServiceReemplazado);
            }
        });
    }

    public async Task SembrarAsync(params string[] marcas)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.MarcasAutos.AddRange(marcas.Select(nombre => new MarcaAuto { Nombre = nombre }));
        await dbContext.SaveChangesAsync();
    }
}
