using System.Net;
using MarcasAutos.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MarcasAutos.Tests.Integration;

public class SwaggerAndHealthTests
{
    [Fact]
    public async Task Healthz_Responde200SinNecesitarBaseDeDatos()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/healthz");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task SwaggerJson_ConSwaggerHabilitadoEnProduction_DocumentaElEndpointYElEsquema()
    {
        using var factory = new ApiFactory { SwaggerHabilitado = true };
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Vehicle Brands API", json, StringComparison.Ordinal);
        Assert.Contains("\"/api/MarcasAutos\"", json, StringComparison.Ordinal);
        Assert.Contains("Obtiene todas las marcas de autos.", json, StringComparison.Ordinal);
        Assert.Contains("\"MarcaAutoDto\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("/healthz", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SwaggerUi_ConSwaggerHabilitadoEnProduction_EsAccesibleYLaRaizRedirige()
    {
        using var factory = new ApiFactory { SwaggerHabilitado = true };
        using var client = factory.CreateClient(new() { AllowAutoRedirect = false });

        using var ui = await client.GetAsync("/swagger/index.html");
        using var raiz = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, ui.StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, raiz.StatusCode);
        Assert.Equal("/swagger", raiz.Headers.Location?.OriginalString);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(false)]
    public async Task Swagger_EnProductionSinHabilitarExplicitamente_NoSePublica(bool? habilitado)
    {
        using var factory = new ApiFactory { SwaggerHabilitado = habilitado };
        using var client = factory.CreateClient();

        using var json = await client.GetAsync("/swagger/v1/swagger.json");
        using var ui = await client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.NotFound, json.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, ui.StatusCode);
    }

    [Fact]
    public async Task Swagger_EnDevelopment_EstaHabilitadoPorDefecto()
    {
        using var factory = new ApiFactory { Entorno = Environments.Development };
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
