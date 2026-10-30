using MarcasAutos.Api.Domain.Entities;
using MarcasAutos.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutos.Tests.Fixtures;

/// <summary>
/// Crea contextos sobre una base InMemory con nombre único, de modo que ningún test comparte datos con otro.
/// Limitación: InMemory no es relacional; migraciones e índices de PostgreSQL se validan contra PostgreSQL real (Docker).
/// </summary>
public static class InMemoryDbContextFactory
{
    public static AppDbContext Create(params string[] marcas)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"marcas-{Guid.NewGuid()}")
            .Options;

        var context = new AppDbContext(options);
        context.MarcasAutos.AddRange(marcas.Select(nombre => new MarcaAuto { Nombre = nombre }));
        context.SaveChanges();
        return context;
    }
}
