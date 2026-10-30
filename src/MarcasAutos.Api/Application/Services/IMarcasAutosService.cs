using MarcasAutos.Api.Application.DTOs;

namespace MarcasAutos.Api.Application.Services;

public interface IMarcasAutosService
{
    Task<IReadOnlyList<MarcaAutoDto>> ObtenerTodasAsync(CancellationToken cancellationToken);
}
