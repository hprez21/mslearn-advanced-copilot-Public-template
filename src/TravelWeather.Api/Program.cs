using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;
using TravelWeather.Api.Documentation;
using TravelWeather.Api.Models;
using TravelWeather.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(WeatherOpenApi.Configure);
builder.Services.AddProblemDetails();

var dataPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "weather.json");
if (!File.Exists(dataPath))
{
    // Permite iniciar el binario publicado desde un directorio de trabajo distinto.
    dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "weather.json");
}
builder.Services.AddSingleton(new WeatherRepository(dataPath));

var app = builder.Build();

app.UseExceptionHandler();
app.MapOpenApi();
app.MapScalarApiReference("/docs", options => options
    .WithTitle("Travel Weather API · Laboratorio de Copilot")
    .WithOpenApiRoutePattern("/openapi/v1.json")
    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));

app.MapGet("/scalar", () => TypedResults.Redirect("/docs/"))
    .ExcludeFromDescription();

app.MapGet("/", () => TypedResults.Redirect("/docs"))
    .ExcludeFromDescription();

app.MapGet("/countries", (WeatherRepository weather) => TypedResults.Ok(weather.GetCountries()))
    .WithName("GetCountries")
    .WithSummary("Lista los países disponibles.");

// TODO del laboratorio: crear GET /countries/{country} con ayuda de Copilot.
// Usar WeatherRepository.TryGetCities. Responder 200 con un arreglo JSON
// ordenado de ciudades, o 404 ProblemDetails si el país no existe.

app.MapGet("/countries/{country}/{city}/{month}",
    Results<Ok<MonthlyWeather>, ProblemHttpResult> (
        string country, string city, string month, WeatherRepository weather) =>
        weather.TryGetMonthlyWeather(country, city, month, out var result)
            ? TypedResults.Ok(result)
            : TypedResults.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Datos de clima no encontrados",
                detail: "No existe esa combinación exacta de país, ciudad y mes."))
    .WithName("GetMonthlyWeather")
    .WithSummary("Consulta las temperaturas alta y baja del mes en grados Fahrenheit.")
    .ProducesProblem(StatusCodes.Status404NotFound);

app.MapGet("/ejemplos", () => TypedResults.Content("""
    <!doctype html>
    <html lang="es">
    <head>
      <meta charset="utf-8">
      <meta name="viewport" content="width=device-width, initial-scale=1">
      <title>Ejemplos · Travel Weather API</title>
      <style>
        :root { color-scheme: light dark; font-family: system-ui, sans-serif; }
        body { max-width: 54rem; margin: 3rem auto; padding: 0 1.25rem; line-height: 1.6; }
        code { overflow-wrap: anywhere; }
        li { margin-block: .75rem; }
        table { width: 100%; border-collapse: collapse; }
        th, td { padding: .65rem; border: 1px solid #888; text-align: left; }
        .table-scroll { overflow-x: auto; }
      </style>
    </head>
    <body>
      <main>
        <h1>Ejemplos para probar la API</h1>
        <p>ASP.NET Core Minimal APIs. Datos locales de temperaturas históricas en grados Fahrenheit.</p>
        <p><a href="/docs/">Abrir Scalar y probar los endpoints</a>.</p>
        <p>Selecciona una operación en Scalar y abre <strong>Test Request</strong>.
          Introduce los valores de la tabla en los parámetros de ruta y pulsa <strong>Send</strong>.
          Son consultas GET: no necesitas insertar un cuerpo JSON.</p>
        <h2>Qué introducir</h2>
        <div class="table-scroll">
        <table>
          <thead><tr><th>Operación</th><th>country</th><th>city</th><th>month</th><th>Resultado esperado</th></tr></thead>
          <tbody>
            <tr><td><code>GET /countries</code></td><td>—</td><td>—</td><td>—</td><td>200, lista de siete países</td></tr>
            <tr><td>Clima mensual</td><td><code>Spain</code></td><td><code>Seville</code></td><td><code>January</code></td><td>200, <code>{"high":61,"low":41}</code></td></tr>
            <tr><td>Clima mensual</td><td><code>England</code></td><td><code>London</code></td><td><code>January</code></td><td>200, <code>{"high":45,"low":36}</code></td></tr>
            <tr><td>Clima mensual</td><td><code>Spain</code></td><td><code>Madrid</code></td><td><code>January</code></td><td>404, ciudad ausente</td></tr>
            <tr><td>Ciudades del país</td><td><code>Spain</code></td><td>—</td><td>—</td><td>200, <code>["Seville"]</code></td></tr>
            <tr><td>Ciudades del país</td><td><code>Portugal</code></td><td>—</td><td>—</td><td>200, <code>["Lisbon","Porto"]</code></td></tr>
            <tr><td>Ciudades del país</td><td><code>Atlantis</code></td><td>—</td><td>—</td><td>404, país ausente</td></tr>
          </tbody>
        </table>
        </div>
        <p>Las filas de ciudades corresponden a <code>GET /countries/{country}</code>,
          la ruta del ejercicio. Si estás preparando una copia sin esa ruta, aparecerá
          en Scalar cuando la implementes y reinicies la aplicación.</p>
        <h2>Enlaces para consultas de clima</h2>
        <ul>
          <li><a href="/countries"><code>GET /countries</code></a>: lista de países.</li>
          <li><a href="/countries/Spain/Seville/January"><code>GET /countries/Spain/Seville/January</code></a>:
            temperaturas de Seville en January (<code>{"high":61,"low":41}</code>).</li>
        </ul>
        <p>Los nombres de países, ciudades y meses distinguen mayúsculas y minúsculas.
          Los meses están en inglés. Una combinación inexistente devuelve 404.</p>
        <h2>Contrato OpenAPI</h2>
        <p><a href="/openapi/v1.json">Abrir el documento OpenAPI generado por la aplicación</a>.</p>
        <p>También puedes probar las solicitudes desde el archivo
          <code>TravelWeather.Api.http</code> o una terminal.</p>
        <p>Al añadir o modificar rutas, reinicia la API y recarga Scalar.
          La interfaz lee las operaciones implementadas desde el contrato OpenAPI.</p>
      </main>
    </body>
    </html>
    """, "text/html; charset=utf-8"))
    .ExcludeFromDescription();

app.Run();

public partial class Program { }
