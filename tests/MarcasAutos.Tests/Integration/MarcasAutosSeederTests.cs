using MarcasAutos.Api.Domain.Entities;
using MarcasAutos.Api.Infrastructure.Seeding;
using MarcasAutos.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutos.Tests.Integration;

public class MarcasAutosSeederTests
{
    private static readonly string[] Esperadas = ["Ford", "Honda", "Toyota"];

    [Fact]
    public async Task SeedAsync_SobreBaseVacia_InsertaToyotaHondaYFord()
    {
        await using var dbContext = InMemoryDbContextFactory.Create();

        await MarcasAutosSeeder.SeedAsync(dbContext, CancellationToken.None);

        Assert.Equal(Esperadas, await NombresAsync(dbContext));
    }

    [Fact]
    public async Task SeedAsync_EjecutadoVariasVeces_NoDuplicaRegistros()
    {
        await using var dbContext = InMemoryDbContextFactory.Create();

        await MarcasAutosSeeder.SeedAsync(dbContext, CancellationToken.None);
        await MarcasAutosSeeder.SeedAsync(dbContext, CancellationToken.None);
        await MarcasAutosSeeder.SeedAsync(dbContext, CancellationToken.None);

        Assert.Equal(Esperadas, await NombresAsync(dbContext));
    }

    [Fact]
    public async Task SeedAsync_ConMarcaExistente_SoloAgregaLasFaltantes()
    {
        await using var dbContext = InMemoryDbContextFactory.Create("Honda");

        await MarcasAutosSeeder.SeedAsync(dbContext, CancellationToken.None);

        Assert.Equal(Esperadas, await NombresAsync(dbContext));
    }

    [Fact]
    public async Task Seed_Sincrono_EsIdempotente()
    {
        await using var dbContext = InMemoryDbContextFactory.Create();

        MarcasAutosSeeder.Seed(dbContext);
        MarcasAutosSeeder.Seed(dbContext);

        Assert.Equal(Esperadas, await NombresAsync(dbContext));
    }

    private static Task<List<string>> NombresAsync(DbContext dbContext) =>
        dbContext.Set<MarcaAuto>().Select(m => m.Nombre).OrderBy(n => n).ToListAsync();
}
