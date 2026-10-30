using Microsoft.OpenApi;

namespace MarcasAutos.Api.Swagger;

public static class SwaggerSetup
{
    private const string EnabledKey = "Swagger:Enabled";

    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Vehicle Brands API",
                Version = "v1",
                Description = "API REST de consulta del catálogo de marcas de autos almacenado en PostgreSQL."
            });

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(SwaggerSetup).Assembly.GetName().Name}.xml"));
        });

        return services;
    }

    /// <summary>
    /// Swagger se publica en Development o cuando Swagger:Enabled=true, sin tener que ejecutar todo el entorno como Development.
    /// </summary>
    public static bool IsSwaggerEnabled(this WebApplication app) =>
        app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>(EnabledKey);

    public static WebApplication UseSwaggerDocs(this WebApplication app)
    {
        if (!app.IsSwaggerEnabled())
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle Brands API v1"));
        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

        return app;
    }
}
