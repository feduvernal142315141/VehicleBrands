using MarcasAutos.Api.Domain.Entities;
using MarcasAutos.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutos.Api.Infrastructure.Seeding;

/// <summary>
/// Carga inicial de marcas. Es idempotente: solo inserta las marcas que aún no existen,
/// por lo que puede ejecutarse en cada arranque sin generar duplicados.
/// </summary>
public static class MarcasAutosSeeder
{
    public static readonly IReadOnlyList<string> MarcasIniciales = ["Toyota", "Honda", "Ford"];

    public static void Seed(DbContext context)
    {
        var existentes = context.Set<MarcaAuto>().Select(m => m.Nombre).ToHashSet();
        AgregarFaltantes(context, existentes);
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
    {
        var existentes = (await context.Set<MarcaAuto>()
            .Select(m => m.Nombre)
            .ToListAsync(cancellationToken)).ToHashSet();
        AgregarFaltantes(context, existentes);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static void AgregarFaltantes(DbContext context, HashSet<string> existentes)
    {
        context.Set<MarcaAuto>().AddRange(MarcasIniciales
            .Where(nombre => !existentes.Contains(nombre))
            .Select(nombre => new MarcaAuto { Nombre = nombre }));
    }
}
