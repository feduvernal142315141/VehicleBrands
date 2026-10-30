using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarcasAutos.Api.Application.DTOs;
using MarcasAutos.Api.Application.Services;
using MarcasAutos.Tests.Fixtures;

namespace MarcasAutos.Tests.Integration;

public class MarcasAutosEndpointTests
{
    [Fact]
    public async Task GetMarcasAutos_ConDatos_Responde200ConElJsonEsperado()
    {
        using var factory = new ApiFactory();
        await factory.SembrarAsync("Toyota", "Honda", "Ford");
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/MarcasAutos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var marcas = await response.Content.ReadFromJsonAsync<List<MarcaAutoDto>>(JsonSerializerOptions.Web);
        Assert.Equal(
            [new MarcaAutoDto(1, "Toyota"), new MarcaAutoDto(2, "Honda"), new MarcaAutoDto(3, "Ford")],
            marcas);
    }

    [Fact]
    public async Task GetMarcasAutos_SerializaPropiedadesEnCamelCase()
    {
        using var factory = new ApiFactory();
        await factory.SembrarAsync("Toyota");
        using var client = factory.CreateClient();

        var json = await client.GetStringAsync("/api/MarcasAutos");

        Assert.Equal("""[{"id":1,"nombre":"Toyota"}]""", json);
    }

    [Fact]
    public async Task GetMarcasAutos_SinDatos_Responde200ConArregloVacio()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/MarcasAutos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("[]", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetMarcasAutos_CuandoElServicioFalla_Responde500SinExponerDetalles()
    {
        using var factory = new ApiFactory { ServiceReemplazado = typeof(ServicioQueFalla) };
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/MarcasAutos");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var cuerpo = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("cadena-secreta", cuerpo, StringComparison.Ordinal);
        Assert.DoesNotContain(nameof(ServicioQueFalla), cuerpo, StringComparison.Ordinal);
    }

    private sealed class ServicioQueFalla : IMarcasAutosService
    {
        public Task<IReadOnlyList<MarcaAutoDto>> ObtenerTodasAsync(CancellationToken cancellationToken) =>
            throw new InvalidOperationException("cadena-secreta");
    }
}
