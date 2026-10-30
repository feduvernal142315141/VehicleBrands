using MarcasAutos.Api.Application.DTOs;
using MarcasAutos.Api.Application.Services;
using MarcasAutos.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarcasAutos.Tests.Controllers;

public class MarcasAutosControllerTests
{
    [Fact]
    public async Task Get_CuandoElServicioDevuelveMarcas_Responde200ConLosDatos()
    {
        IReadOnlyList<MarcaAutoDto> esperado = [new(1, "Toyota"), new(2, "Honda"), new(3, "Ford")];
        var controller = new MarcasAutosController(new ServicioFijo(esperado));

        var resultado = await controller.Get(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Equal(esperado, ok.Value);
    }

    [Fact]
    public async Task Get_SinMarcas_Responde200ConColeccionVacia()
    {
        var controller = new MarcasAutosController(new ServicioFijo([]));

        var resultado = await controller.Get(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Empty(Assert.IsAssignableFrom<IReadOnlyList<MarcaAutoDto>>(ok.Value));
    }

    private sealed class ServicioFijo(IReadOnlyList<MarcaAutoDto> marcas) : IMarcasAutosService
    {
        public Task<IReadOnlyList<MarcaAutoDto>> ObtenerTodasAsync(CancellationToken cancellationToken) =>
            Task.FromResult(marcas);
    }
}
