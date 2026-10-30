using MarcasAutos.Api.Application.DTOs;
using MarcasAutos.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarcasAutos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarcasAutosController(IMarcasAutosService marcasAutosService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<MarcaAutoDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MarcaAutoDto>>> Get(CancellationToken cancellationToken)
    {
        var marcas = await marcasAutosService.ObtenerTodasAsync(cancellationToken);
        return Ok(marcas);
    }
}
