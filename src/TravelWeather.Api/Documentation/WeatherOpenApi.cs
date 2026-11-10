using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TravelWeather.Api.Documentation;

public static class WeatherOpenApi
{
    public static void Configure(OpenApiOptions options)
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info.Title = "Travel Weather API";
            document.Info.Description = """
                API del laboratorio de GitHub Copilot con C# y ASP.NET Core Minimal APIs.

                **Cómo probar:** selecciona una operación y abre **Test Request**.
                Completa los parámetros de ruta y pulsa **Send**. Las consultas son GET;
                **no requieren un cuerpo JSON**.

                **Ejemplo de clima:** `country = Spain`, `city = Seville`, `month = January`.
                Respuesta: `{"high":61,"low":41}`. Las temperaturas se interpretan como °F.

                **Otro ejemplo:** `England`, `London`, `January` devuelve `{"high":45,"low":36}`.
                Para comprobar un 404, usa `Spain`, `Madrid`, `January`.

                Los nombres distinguen mayúsculas y minúsculas. Los meses están en inglés.
                La operación de ciudades se muestra cuando está implementada en la aplicación.
                [Ver la tabla completa de ejemplos](/ejemplos).
                """;
            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, _) =>
        {
            foreach (var parameter in operation.Parameters ?? [])
            {
                if (parameter is not OpenApiParameter input)
                {
                    continue;
                }

                var example = input.Name switch
                {
                    "country" => "Spain",
                    "city" => "Seville",
                    "month" => "January",
                    _ => null
                };
                if (example is null)
                {
                    continue;
                }

                input.Example = JsonValue.Create(example);
                input.Description = input.Name switch
                {
                    "country" => "Clave exacta del país. Ejemplo: Spain. Disponibles: England, France, Germany, Italy, Peru, Portugal, Spain.",
                    "city" => "Ciudad del país seleccionado. Spain: Seville; England: London; Portugal: Lisbon o Porto. Madrid no está en los datos.",
                    _ => "Mes en inglés y con mayúscula inicial: January, February, March, April, May, June, July, August, September, October, November, December."
                };
            }

            var path = context.Description.RelativePath?.TrimStart('/');
            var successExample = path switch
            {
                "countries" => """["England","France","Germany","Italy","Peru","Portugal","Spain"]""",
                "countries/{country}" => """["Seville"]""",
                "countries/{country}/{city}/{month}" => """{"high":61,"low":41}""",
                _ => null
            };
            if (successExample is not null
                && operation.Responses?.TryGetValue("200", out var response) == true
                && response.Content?.TryGetValue("application/json", out var json) == true)
            {
                json.Example = JsonNode.Parse(successExample);
            }

            return Task.CompletedTask;
        });
    }
}
