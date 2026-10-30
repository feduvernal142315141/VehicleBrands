using MarcasAutos.Api.Application.DTOs;
using MarcasAutos.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarcasAutos.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class MarcasAutosController(IMarcasAutosService marcasAutosService) : ControllerBase
{
    /// <summary>Obtiene todas las marcas de autos.</summary>
    /// <remarks>Las marcas se devuelven ordenadas por identificador. Si no existen, se devuelve un arreglo vacío.</remarks>
    /// <param name="cancellationToken">Cancela la consulta si el cliente cierra la conexión.</param>
    /// <response code="200">Listado de marcas de autos.</response>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<MarcaAutoDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MarcaAutoDto>>> Get(CancellationToken cancellationToken)
    {
        var marcas = await marcasAutosService.ObtenerTodasAsync(cancellationToken);
        return Ok(marcas);
    }
}
