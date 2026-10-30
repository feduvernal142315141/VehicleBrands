using MarcasAutos.Api.Infrastructure;
using MarcasAutos.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarcasAutos.Tests.Integration;

public class InfrastructureRegistrationTests
{
    [Fact]
    public void AppDbContext_SinCadenaDeConexion_FallaIndicandoLaClaveRequerida()
    {
        using var provider = BuildProvider(new Dictionary<string, string?>());
        using var scope = provider.CreateScope();

        var ex = Assert.Throws<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<AppDbContext>());

        Assert.Contains("ConnectionStrings:Postgres", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AppDbContext_ConCadenaDeConexion_SeResuelveSinConectar()
    {
        using var provider = BuildProvider(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres"] = "Host=host-inexistente;Database=db;Username=u;Password=p"
        });
        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", dbContext.Database.ProviderName);
    }

    [Fact]
    public void Modelo_MapeaMarcaAutoALaTablaMarcasAutosConNombreUnicoYObligatorio()
    {
        using var provider = BuildProvider(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres"] = "Host=host-inexistente;Database=db;Username=u;Password=p"
        });
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entidad = dbContext.Model.FindEntityType(typeof(MarcasAutos.Api.Domain.Entities.MarcaAuto));

        Assert.NotNull(entidad);
        Assert.Equal("MarcasAutos", entidad.GetTableName());
        var nombre = entidad.FindProperty(nameof(MarcasAutos.Api.Domain.Entities.MarcaAuto.Nombre));
        Assert.NotNull(nombre);
        Assert.False(nombre.IsNullable);
        Assert.Contains(entidad.GetIndexes(), i => i.IsUnique && i.Properties.Single() == nombre);
    }

    private static ServiceProvider BuildProvider(Dictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddInfrastructure(configuration);
        return services.BuildServiceProvider();
    }
}
