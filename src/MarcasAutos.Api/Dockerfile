# Contexto de build: raíz del repositorio (ver docker-compose.yml).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Se copian primero solo los manifiestos para aprovechar la caché de capas en el restore.
COPY Directory.Build.props ./
COPY src/MarcasAutos.Api/MarcasAutos.Api.csproj src/MarcasAutos.Api/
RUN dotnet restore src/MarcasAutos.Api/MarcasAutos.Api.csproj

COPY src/MarcasAutos.Api/ src/MarcasAutos.Api/
RUN dotnet publish src/MarcasAutos.Api/MarcasAutos.Api.csproj \
    --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Usuario no privilegiado incluido en las imágenes oficiales de ASP.NET.
USER $APP_UID
ENTRYPOINT ["dotnet", "MarcasAutos.Api.dll"]
