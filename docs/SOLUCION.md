# Solución de referencia para el instructor

Utiliza esta referencia al preparar la sesión o después de que el grupo haya trabajado con Copilot. El proyecto inicial deja `GET /countries/{country}` pendiente para realizarlo durante la práctica. Estos fragmentos no se aplican automáticamente.

## 1. Endpoint de ciudades

En `src/TravelWeather.Api/Program.cs`, sustituye el TODO de la ruta nueva por este bloque, antes de `app.Run()`. El archivo ya importa `Microsoft.AspNetCore.Http.HttpResults` y `TravelWeather.Api.Services`.

```csharp
app.MapGet("/countries/{country}",
    Results<Ok<string[]>, ProblemHttpResult> (
        string country, WeatherRepository weather) =>
        weather.TryGetCities(country, out var cities)
            ? TypedResults.Ok(cities)
            : TypedResults.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "País no encontrado",
                detail: "No existe un país con esa clave exacta."))
    .WithName("GetCities")
    .WithSummary("Lista las ciudades disponibles de un país.")
    .ProducesProblem(StatusCodes.Status404NotFound);
```

`TryGetCities` ya ordena con `StringComparer.Ordinal` y devuelve una copia del arreglo. Su anotación `NotNullWhen(true)` permite que el compilador identifique que `cities` tiene valor en la rama correcta. No hace falta volver a cargar el JSON ni traducir las claves.

| Solicitud | Código | Cuerpo esperado |
|---|---:|---|
| `GET /countries/Spain` | 200 | `["Seville"]` |
| `GET /countries/Portugal` | 200 | `["Lisbon","Porto"]` |
| `GET /countries/Atlantis` | 404 | ProblemDetails con `status: 404` |
| `GET /countries/spain` | 404 | ProblemDetails con `status: 404` |
| `POST /countries/Spain` | 405 | No se exige un cuerpo concreto |

La ruta de ciudades no recibe un mes ni devuelve temperaturas. La consulta de clima sigue en `GET /countries/{country}/{city}/{month}`. Los valores se interpretan como Fahrenheit en esta adaptación y se conservan sin conversión.

## 2. Pruebas de integración

Crea `tests/TravelWeather.Api.Tests/CitiesEndpointTests.cs`. El proyecto de pruebas ya contiene las referencias a xUnit y `Microsoft.AspNetCore.Mvc.Testing`; no es necesario agregar paquetes para este ejercicio.

```csharp
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TravelWeather.Api.Tests;

public sealed class CitiesEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;

    public CitiesEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Spain_ReturnsOnlySeville()
    {
        using var response = await _client.GetAsync("/countries/Spain");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json",
            response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(new[] { "Seville" },
            await response.Content.ReadFromJsonAsync<string[]>());
    }

    [Fact]
    public async Task Portugal_ReturnsItsCitiesInOrdinalOrder()
    {
        using var response = await _client.GetAsync("/countries/Portugal");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new[] { "Lisbon", "Porto" },
            await response.Content.ReadFromJsonAsync<string[]>());
    }

    [Theory]
    [InlineData("Atlantis")]
    [InlineData("spain")]
    public async Task UnknownCountry_Returns404ProblemDetails(string country)
    {
        using var response = await _client.GetAsync($"/countries/{country}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
        using var body = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        Assert.Equal(404, body.RootElement.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(
            body.RootElement.GetProperty("title").GetString()));
    }

    [Fact]
    public async Task Post_IsNotAllowed()
    {
        using var response = await _client.PostAsync("/countries/Spain", null);

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [Fact]
    public async Task OpenApi_ContainsTheCitiesRoute()
    {
        using var response = await _client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var body = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        var operation = body.RootElement.GetProperty("paths")
            .GetProperty("/countries/{country}").GetProperty("get");
        var responses = operation.GetProperty("responses");
        Assert.True(responses.TryGetProperty("200", out _));
        Assert.True(responses.TryGetProperty("404", out _));
    }

    public void Dispose() => _client.Dispose();
}
```

Ejecuta desde la raíz:

```text
dotnet test TravelWeather.sln
```

`WebApplicationFactory` inicia la aplicación en el servidor de pruebas. Esta ejecución no usa el puerto 5050 ni necesita una terminal con `dotnet run`. Las pruebas nuevas deben coexistir con las iniciales.

## 3. Revisar la documentación creada con Agent

El documento solicitado durante la práctica es `docs/USO-API.md`. Su contenido puede variar, pero debe permitir reproducir el uso real del proyecto:

- SDK .NET 10 y comandos `dotnet restore TravelWeather.sln`, `dotnet build TravelWeather.sln`, `dotnet run --project src/TravelWeather.Api --no-launch-profile --urls http://localhost:5050` y `dotnet test TravelWeather.sln`, ejecutados desde la raíz.
- Dirección `http://localhost:5050`, establecida explícitamente en el comando de ejecución.
- Las tres rutas de datos, con ejemplos tomados del JSON existente y el error de país ausente.
- Claves exactas en inglés, comparación sensible a mayúsculas y la interpretación de temperaturas en Fahrenheit.
- Scalar en `/docs`, los valores de entrada de `/ejemplos` y el contrato generado en `/openapi/v1.json`.
- Pasos para abrir **Test Request**, completar `country`, `city` y `month`, pulsar **Send** y revisar el estado y la respuesta. Los GET no requieren cuerpo JSON.

Conserva Scalar en `/docs`. Para ampliar la ayuda de `/ejemplos`, puedes añadir estos enlaces a la lista de rutas y el párrafo que explica el caso ausente, si aún no están presentes:

```html
<li><a href="/countries/Spain"><code>GET /countries/Spain</code></a>:
  ciudades del país (<code>["Seville"]</code>).</li>
<li><a href="/countries/Portugal"><code>GET /countries/Portugal</code></a>:
  ciudades del país (<code>["Lisbon","Porto"]</code>).</li>
```

```html
<p>Si el país no existe, la ruta devuelve 404 con ProblemDetails.
  Las claves distinguen mayúsculas y minúsculas.</p>
```

Reinicia la API después de modificar el código, recarga Scalar y ejecuta los ejemplos de ciudades. Comprueba también los ejemplos de parámetros y respuestas del documento OpenAPI.
