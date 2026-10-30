using MarcasAutos.Api.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace MarcasAutos.Tests.Integration;

public class DatabaseInitializerTests
{
    [Fact]
    public async Task StartAsync_ConMigracionesDeshabilitadas_NoCreaScopeNiTocaLaBaseDeDatos()
    {
        // Sin AppDbContext registrado: si intentara migrar, el scope fallaría al resolverlo.
        await using var provider = new ServiceCollection().BuildServiceProvider();
        var initializer = new DatabaseInitializer(
            provider.GetRequiredService<IServiceScopeFactory>(),
            Options.Create(new DatabaseOptions { ApplyMigrationsOnStartup = false }),
            NullLogger<DatabaseInitializer>.Instance);

        await initializer.StartAsync(CancellationToken.None);
        await initializer.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_ConMigracionesHabilitadasSinContexto_PropagaElError()
    {
        await using var provider = new ServiceCollection().BuildServiceProvider();
        var initializer = new DatabaseInitializer(
            provider.GetRequiredService<IServiceScopeFactory>(),
            Options.Create(new DatabaseOptions { ApplyMigrationsOnStartup = true }),
            NullLogger<DatabaseInitializer>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() => initializer.StartAsync(CancellationToken.None));
    }
}
