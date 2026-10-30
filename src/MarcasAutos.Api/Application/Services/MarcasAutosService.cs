using MarcasAutos.Api.Application.DTOs;
using MarcasAutos.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutos.Api.Application.Services;

public sealed class MarcasAutosService(AppDbContext dbContext) : IMarcasAutosService
{
    public async Task<IReadOnlyList<MarcaAutoDto>> ObtenerTodasAsync(CancellationToken cancellationToken)
    {
        // Proyección directa al DTO: lectura sin tracking y solo las columnas necesarias.
        // El orden por Id garantiza una respuesta determinista.
        return await dbContext.MarcasAutos
            .AsNoTracking()
            .OrderBy(m => m.Id)
            .Select(m => new MarcaAutoDto(m.Id, m.Nombre))
            .ToListAsync(cancellationToken);
    }
}
