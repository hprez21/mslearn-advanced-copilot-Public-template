# GitHub Copilot avanzado con C# y .NET

Laboratorio guiado para practicar GitHub Copilot sobre una API de clima de viajes con **ASP.NET Core Minimal APIs y .NET 10**. Se trabaja en el equipo local y el instructor conduce el entrenamiento.

Adaptación de [MicrosoftDocs/mslearn-advanced-copilot](https://github.com/MicrosoftDocs/mslearn-advanced-copilot). La API y las pruebas están en C#; los datos de clima se conservan del repositorio original. Esta es una adaptación para clase, con cambios documentados en [NOTICE.md](NOTICE.md).

## Empezar

1. Instala el [SDK de .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0), [Git](https://git-scm.com/downloads) y [Visual Studio Code](https://code.visualstudio.com/).
2. En VS Code instala **C# Dev Kit** y habilita **GitHub Copilot** con una cuenta que tenga acceso al servicio. La extensión REST Client es opcional.
3. Descarga el repositorio que comparta el instructor, o clónalo con su URL y abre la carpeta local en VS Code. Abre una terminal en la raíz, junto a `TravelWeather.sln`.
4. Ejecuta:

   ```sh
   dotnet --version
   dotnet restore TravelWeather.sln
   dotnet build TravelWeather.sln --no-restore
   dotnet test TravelWeather.sln --no-build
   dotnet run --project src/TravelWeather.Api --no-launch-profile --urls http://localhost:5050
   ```

5. Abre [http://localhost:5050/docs](http://localhost:5050/docs) para explorar y probar los endpoints con **Scalar**. En [http://localhost:5050/ejemplos](http://localhost:5050/ejemplos) tienes una tabla de valores para introducir. Detén la API con **Ctrl+C** cuando necesites volver a compilar o ejecutar pruebas.

El primer comando debe seleccionar un SDK estable `10.0.x`. `global.json` admite las bandas estables del SDK 10 y evita elegir una versión preliminar. Necesitas conexión para descargar herramientas, restaurar paquetes y usar Copilot; las consultas de clima usan el archivo local.

## Recorrido con el instructor

Sigue la [guía del participante](docs/GUIA.md), con pasos, prompts y resultados esperados:

| Paso | Trabajo con Copilot | Resultado |
|---|---|---|
| 1 | Inline Chat y contexto del código | Nueva ruta `GET /countries/{country}` |
| 2 | Chat para generar y corregir pruebas | Pruebas de integración con xUnit |
| 3 | Agente para documentar el proyecto | `docs/USO-API.md` con instrucciones locales |

La ruta de ciudades **queda pendiente en esta versión inicial**. El objetivo es construirla durante la práctica. El proyecto compila y sus pruebas base pueden ejecutarse antes de comenzar. La revisión del ejercicio la realiza el instructor; `dotnet test` comprueba el comportamiento del software.

## API del laboratorio

| Método y ruta | Respuesta |
|---|---|
| `GET /` | Redirección a `/docs` |
| `GET /docs` | Scalar: documentación visual y envío de solicitudes |
| `GET /scalar` | Redirección a `/docs` |
| `GET /ejemplos` | Ayuda local con valores para introducir |
| `GET /openapi/v1.json` | Contrato OpenAPI generado a partir de los endpoints |
| `GET /countries` | Arreglo de los siete países disponibles |
| `GET /countries/{country}` | Arreglo de ciudades, cuando la ruta del ejercicio está implementada |
| `GET /countries/{country}/{city}/{month}` | Objeto con `high` y `low`, o 404 ProblemDetails |

Ejemplo: `/countries/Spain/Seville/January` devuelve `{"high":61,"low":41}`. Las claves de los datos distinguen mayúsculas y minúsculas; los meses están en inglés. La adaptación interpreta las temperaturas como Fahrenheit sin modificar los números originales. Son datos del ejercicio, no una consulta al clima actual.

En Scalar, selecciona una operación, abre **Test Request** y completa sus parámetros de ruta. Para consultar clima introduce `country = Spain`, `city = Seville` y `month = January`, y pulsa **Send**: la respuesta es `200` con `{"high":61,"low":41}`. Para listar ciudades usa `country = Spain` y espera `["Seville"]` cuando esa ruta esté implementada.

Todos los endpoints de datos usan **GET**: se introducen valores en los parámetros de la ruta y no se envía un cuerpo JSON. El contrato técnico está en `/openapi/v1.json`. También puedes usar el archivo [TravelWeather.Api.http](src/TravelWeather.Api/TravelWeather.Api.http), REST Client o una terminal. El entorno local usa HTTP en `localhost` para simplificar la preparación.

## Archivos principales

```text
TravelWeather.sln
global.json
src/TravelWeather.Api/
  Program.cs                     # Minimal APIs y configuración de Scalar
  Data/weather.json              # Datos originales
  Models/MonthlyWeather.cs
  Services/WeatherRepository.cs
  TravelWeather.Api.http         # Solicitudes locales
tests/TravelWeather.Api.Tests/
  BaselineApiTests.cs             # Pruebas iniciales
docs/
  GUIA.md                        # Pasos del participante
  INSTRUCTOR.md                  # Preparación y conducción de clase
  SOLUCION.md                    # Referencia para el instructor
  PUBLICAR-EN-GITHUB.md           # Publicación de la versión inicial
FUENTES_CONTENIDO.md
NOTICE.md
LICENSE
LICENSE-CODE
```

Las instrucciones de Copilot están en `.github/copilot-instructions.md` y `.github/instructions/csharp.instructions.md`. Las tareas de VS Code permiten compilar, iniciar la API y ejecutar las pruebas desde **Terminal > Run Task**.

## Publicación

Consulta [cómo publicar esta versión inicial en GitHub](docs/PUBLICAR-EN-GITHUB.md). GitHub aloja el código y las guías; cada participante ejecuta la API en su equipo. No hay configuración de Codespaces ni flujos de calificación automática.

Documentación bajo [CC BY 4.0](LICENSE) y código bajo [MIT](LICENSE-CODE), con la procedencia y los cambios en [NOTICE.md](NOTICE.md).
