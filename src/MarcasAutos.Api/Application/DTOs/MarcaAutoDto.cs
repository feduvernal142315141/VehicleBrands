namespace MarcasAutos.Api.Application.DTOs;

/// <summary>Marca de auto.</summary>
/// <param name="Id">Identificador único de la marca.</param>
/// <param name="Nombre">Nombre de la marca.</param>
public sealed record MarcaAutoDto(int Id, string Nombre);
