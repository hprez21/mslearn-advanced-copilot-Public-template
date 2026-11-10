# Contexto del laboratorio

Este repositorio es un laboratorio guiado de GitHub Copilot con C#, .NET 10 y ASP.NET Core Minimal APIs. El instructor decide cuándo iniciar cada ejercicio.

- Lee el código y `Data/weather.json` antes de proponer cambios. No inventes países, ciudades ni valores.
- Mantén las rutas existentes y los contratos JSON. Usa Minimal APIs y la inyección de `WeatherRepository`; evita controllers, bases de datos y servicios externos para este ejercicio.
- Los identificadores del archivo JSON distinguen mayúsculas y minúsculas. Spain solo contiene Seville. Las temperaturas se interpretan como Fahrenheit en esta adaptación.
- Devuelve 404 con ProblemDetails cuando los datos solicitados no existan. No conviertas esos casos en respuestas 500.
- Limita los cambios al ejercicio solicitado. Explica el cambio y su evidencia local antes de darlo por terminado.
- Usa xUnit y `WebApplicationFactory<Program>` para las pruebas de integración HTTP. No debilites las pruebas para obtener un resultado verde.
- Ejecuta `dotnet build TravelWeather.sln` y las pruebas pertinentes cuando tengas herramientas disponibles. Si no puedes ejecutarlas, indícalo.
- Redacta explicaciones y documentación en español; conserva nombres de código y claves JSON en inglés.
- La documentación que se genera durante el ejercicio se guarda en `docs/USO-API.md`. Conserva el README y la guía del participante.
- Conserva Scalar en `/docs` y la ayuda en `/ejemplos`. Mantén ejemplos de parámetros y respuestas en OpenAPI usando datos reales; estos GET no reciben cuerpo JSON.
- El entrenamiento se realiza en el equipo local. No agregues Codespaces, devcontainers, evaluadores de progreso, acciones de calificación ni despliegues.
