using MarcasAutos.Api.Application.Services;
using MarcasAutos.Tests.Fixtures;

namespace MarcasAutos.Tests.Services;

public class MarcasAutosServiceTests
{
    [Fact]
    public async Task ObtenerTodasAsync_ConTresMarcas_DevuelveLasTres()
    {
        await using var dbContext = InMemoryDbContextFactory.Create("Toyota", "Honda", "Ford");
        var service = new MarcasAutosService(dbContext);

        var resultado = await service.ObtenerTodasAsync(CancellationToken.None);

        Assert.Equal(3, resultado.Count);
    }

    [Fact]
    public async Task ObtenerTodasAsync_DevuelveNombresEnOrdenDeInsercion()
    {
        await using var dbContext = InMemoryDbContextFactory.Create("Toyota", "Honda", "Ford");
        var service = new MarcasAutosService(dbContext);

        var resultado = await service.ObtenerTodasAsync(CancellationToken.None);

        Assert.Equal(["Toyota", "Honda", "Ford"], resultado.Select(m => m.Nombre));
        Assert.Equal([1, 2, 3], resultado.Select(m => m.Id));
    }

    [Fact]
    public async Task ObtenerTodasAsync_SinRegistros_DevuelveColeccionVacia()
    {
        await using var dbContext = InMemoryDbContextFactory.Create();
        var service = new MarcasAutosService(dbContext);

        var resultado = await service.ObtenerTodasAsync(CancellationToken.None);

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObtenerTodasAsync_ConContextosDistintos_NoComparteDatos()
    {
        await using var conDatos = InMemoryDbContextFactory.Create("Toyota");
        await using var sinDatos = InMemoryDbContextFactory.Create();

        var resultadoConDatos = await new MarcasAutosService(conDatos).ObtenerTodasAsync(CancellationToken.None);
        var resultadoSinDatos = await new MarcasAutosService(sinDatos).ObtenerTodasAsync(CancellationToken.None);

        Assert.Single(resultadoConDatos);
        Assert.Empty(resultadoSinDatos);
    }

    [Fact]
    public async Task ObtenerTodasAsync_ConTokenCancelado_LanzaOperationCanceledException()
    {
        await using var dbContext = InMemoryDbContextFactory.Create("Toyota");
        var service = new MarcasAutosService(dbContext);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.ObtenerTodasAsync(cts.Token));
    }
}
