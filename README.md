# Marcas de Autos – API Backend

API REST en **ASP.NET Core (.NET 10)** que expone el catálogo de marcas de autos almacenado en **PostgreSQL** mediante **Entity Framework Core 10** (Npgsql). Incluye migraciones, seed idempotente, pruebas xUnit con cobertura mínima del 70 % y entorno Docker Compose.

## Requisitos previos

- Docker y Docker Compose v2 (para la ejecución recomendada).
- .NET SDK 10 (solo para ejecución local y pruebas fuera de Docker).

## Estructura del proyecto

```text
src/MarcasAutos.Api/
  Controllers/                 Endpoints HTTP
  Application/                 Servicios y DTOs
  Domain/Entities/             Entidad MarcaAuto
  Infrastructure/
    Persistence/               AppDbContext, configuraciones EF, migraciones, inicializador
    Seeding/                   Carga inicial idempotente
  Dockerfile
tests/MarcasAutos.Tests/       Pruebas xUnit (servicio, controlador, API en memoria, seed)
docker-compose.yml             PostgreSQL + API
```

## Ejecución con Docker

```bash
docker compose up --build
```

Levanta PostgreSQL (con healthcheck) y la API; esta aplica la migración y el seed al iniciar. No requiere pasos manuales. Las credenciales por defecto son **solo de desarrollo**; pueden cambiarse copiando `.env.example` a `.env`. Si el puerto 8080 está ocupado, defina `API_PORT` (por ejemplo `API_PORT=8081`).

Para detener: `docker compose down` (añada `-v` para borrar también el volumen de datos).

## Endpoint

```http
GET http://localhost:8080/api/MarcasAutos
```

Ejemplo de respuesta (`200 OK`; `[]` si no hay marcas):

```json
[
  { "id": 1, "nombre": "Toyota" },
  { "id": 2, "nombre": "Honda" },
  { "id": 3, "nombre": "Ford" }
]
```

## Swagger y health check

Con Docker Compose, Swagger queda disponible en `http://localhost:8080/swagger` (documento OpenAPI en `/swagger/v1/swagger.json`); desde allí se puede ejecutar el endpoint con **Try it out → Execute**. Se habilita en Development o cuando `Swagger__Enabled=true`, sin necesidad de ejecutar la aplicación como Development.

`GET /healthz` devuelve `200 Healthy` cuando la aplicación está lista. No consulta la base de datos: las migraciones se aplican antes de que el servidor acepte tráfico.

## Despliegue en Render + Neon

Servicio Docker con `Dockerfile Path = src/MarcasAutos.Api/Dockerfile`, `Docker Build Context = .` y `Health Check Path = /healthz`. Variables de entorno:

| Variable | Valor |
|---|---|
| `ConnectionStrings__Postgres` | `Host=<host-directo>.neon.tech;Port=5432;Database=<db>;Username=<usuario>;Password=<password>;SSL Mode=VerifyFull` |
| `Database__ApplyMigrationsOnStartup` | `true` |
| `Swagger__Enabled` | `true` |
| `PORT` | `8080` |

Notas:

- Npgsql no acepta URLs `postgresql://`; use el formato clave=valor anterior.
- Use el host **directo** de Neon (sin `-pooler`): las migraciones usan bloqueos de sesión que no son compatibles con el pooler en modo transacción.
- La imagen escucha en `0.0.0.0:8080` (`ASPNETCORE_HTTP_PORTS=8080`); ASP.NET Core no interpreta `PORT`, por lo que debe mantenerse en `8080` para que Render use el mismo puerto.
- Con el plan gratuito, la primera petición tras la inactividad puede tardar mientras Render y Neon se reactivan; la API reintenta fallos transitorios de conexión.

## Ejecución local (sin Docker para la API)

Requiere un PostgreSQL accesible (por ejemplo `docker compose up -d postgres`, que no publica el puerto; añada un mapeo `5432:5432` si lo necesita).

```bash
dotnet restore
export ConnectionStrings__Postgres="Host=localhost;Port=5432;Database=marcas_autos;Username=marcas_user;Password=<password>"
dotnet run --project src/MarcasAutos.Api
```

La API queda en `http://localhost:8080`.

## Migraciones

- **Docker/arranque:** la API aplica las migraciones pendientes al iniciar (`Database:ApplyMigrationsOnStartup=true`) y, a continuación, el seed. Sobre una base vacía se crea la tabla `MarcasAutos`.
- **Seed:** `MarcasAutosSeeder` se ejecuta con el mecanismo oficial `UseSeeding`/`UseAsyncSeeding` al final de la migración. Solo inserta las marcas que faltan (Toyota, Honda, Ford), por lo que reiniciar no duplica datos.
- **Manual** (herramienta `dotnet-ef` fijada en `dotnet-tools.json`):

```bash
dotnet tool restore
export ConnectionStrings__Postgres="Host=localhost;Database=marcas_autos;Username=marcas_user;Password=<password>"
dotnet ef migrations add NombreMigracion --project src/MarcasAutos.Api --output-dir Infrastructure/Persistence/Migrations
dotnet ef database update --project src/MarcasAutos.Api
```

## Pruebas

```bash
dotnet test --configuration Release
```

Las pruebas usan `Microsoft.EntityFrameworkCore.InMemory` y `WebApplicationFactory`. InMemory no es relacional: migraciones, índices y restricciones de PostgreSQL se validan ejecutando el entorno Docker.

## Cobertura

```bash
dotnet test --configuration Release /p:CollectCoverage=true
```

Usa Coverlet (MSBuild) con umbral del **70 % de líneas**: el comando falla si no se alcanza. Se excluyen únicamente las migraciones generadas. Los reportes (Cobertura y OpenCover) se escriben en `coverage/`.

## Variables de entorno

| Variable | Descripción | Valor por defecto (Compose) |
|---|---|---|
| `ConnectionStrings__Postgres` | Cadena de conexión de la API (obligatoria) | Construida desde las variables siguientes |
| `Database__ApplyMigrationsOnStartup` | Aplica migraciones y seed al iniciar | `true` |
| `POSTGRES_DB` / `POSTGRES_USER` / `POSTGRES_PASSWORD` | Base de datos y credenciales de PostgreSQL | `marcas_autos` / `marcas_user` / `dev_only_change_me` |
| `Swagger__Enabled` | Publica Swagger fuera de Development | `true` en Compose |
| `API_PORT` | Puerto publicado de la API | `8080` |
