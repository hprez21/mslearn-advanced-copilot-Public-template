# Fuentes y correspondencia de la adaptación

Consulta: **9 de noviembre de 2026**, para la versión inicial 0.1.0. Los enlaces de esta página son referencias; las instrucciones de la práctica están completas en `docs/GUIA.md`.

## Repositorio original

Base: [MicrosoftDocs/mslearn-advanced-copilot](https://github.com/MicrosoftDocs/mslearn-advanced-copilot/tree/978400834b44ec00ef2e2bf89b5013186939dfaf), commit `978400834b44ec00ef2e2bf89b5013186939dfaf`.

| Archivo original | Uso en esta entrega |
|---|---|
| [README.md](https://github.com/MicrosoftDocs/mslearn-advanced-copilot/blob/978400834b44ec00ef2e2bf89b5013186939dfaf/README.md) | Secuencia de tres ejercicios y caso Spain/Seville; guía reescrita para C# y entorno local. |
| [main.py](https://github.com/MicrosoftDocs/mslearn-advanced-copilot/blob/978400834b44ec00ef2e2bf89b5013186939dfaf/main.py) | Contratos iniciales de países, clima mensual y acceso a documentación; reimplementados en Minimal APIs. |
| [weather.json](https://github.com/MicrosoftDocs/mslearn-advanced-copilot/blob/978400834b44ec00ef2e2bf89b5013186939dfaf/weather.json) | Datos conservados en `src/TravelWeather.Api/Data/weather.json`. |
| [test_main.py](https://github.com/MicrosoftDocs/mslearn-advanced-copilot/blob/978400834b44ec00ef2e2bf89b5013186939dfaf/test_main.py) | Punto de partida para las pruebas locales; se amplían casos HTTP y errores. |
| [.github/instructions/python.instructions.md](https://github.com/MicrosoftDocs/mslearn-advanced-copilot/blob/978400834b44ec00ef2e2bf89b5013186939dfaf/.github/instructions/python.instructions.md) | Idea de instrucciones por lenguaje, sustituida por convenciones de C#. |
| LICENSE y LICENSE-CODE | Avisos y textos de licencia conservados. Ver [NOTICE.md](NOTICE.md). |

## Unidades de Microsoft Learn consultadas

Módulo: [Advanced GitHub Copilot](https://learn.microsoft.com/en-us/training/modules/advanced-github-copilot/).

| Unidad | Aplicación en la adaptación |
|---|---|
| [1. Introducción](https://learn.microsoft.com/en-us/training/modules/advanced-github-copilot/1-introduction) | Objetivos de generación, pruebas y documentación asistidas. |
| [2. Funciones avanzadas](https://learn.microsoft.com/en-us/training/modules/advanced-github-copilot/2-advanced-github-copilot-features) | Uso de contexto e interacción con Copilot. |
| [3. Preparación](https://learn.microsoft.com/en-us/training/modules/advanced-github-copilot/3-exercise-setup) | Reemplazo completo del entorno remoto por SDK .NET 10 y editor local. |
| [4. Técnicas aplicadas](https://learn.microsoft.com/en-us/training/modules/advanced-github-copilot/4-applied-copilot-techniques) | Iteración sobre propuestas y revisión de resultados. |
| [5. Actualizar una API](https://learn.microsoft.com/en-us/training/modules/advanced-github-copilot/5-exercise-update-a-web-api) | Correspondencia principal: ruta con Inline Chat, pruebas con Chat, documentación con Agent. |
| [6. Comprobación de conocimientos](https://learn.microsoft.com/en-us/training/modules/advanced-github-copilot/6-knowledge-check) | Referencia del módulo; no se integra un calificador ni se copia una evaluación oficial a esta práctica. |
| [7. Resumen](https://learn.microsoft.com/en-us/training/modules/advanced-github-copilot/7-summary) | Cierre guiado del instructor; no se necesita eliminar recursos de Codespaces. |

El enunciado original mezcla listar ciudades y consultar clima. Aquí se separan `GET /countries/{country}` y `GET /countries/{country}/{city}/{month}`. Los tiempos de la guía del instructor son una propuesta de clase propia.

## Documentación técnica

| Fuente primaria | Decisión o comprobación |
|---|---|
| [Política de soporte de .NET](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core) | Selección de .NET 10 estable LTS. |
| [Descarga de .NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) | SDK para el entorno local. |
| [Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0) | `MapGet`, tipos de resultados e inyección de dependencias. |
| [Generar OpenAPI](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-10.0) | `AddOpenApi` y `MapOpenApi`; documento generado en tiempo de ejecución. |
| [Integración oficial de Scalar con ASP.NET Core](https://github.com/scalar/scalar/blob/main/documentation/integrations/aspnetcore/integration.md) | `Scalar.AspNetCore`, interfaz en `/docs` y lectura del documento OpenAPI local. |
| [Pruebas de integración](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0) | Pruebas HTTP con `WebApplicationFactory<Program>`. |
| [Referencia de funciones de IA de VS Code](https://code.visualstudio.com/docs/agents/reference/ai-features-cheat-sheet) | Prompts equivalentes cuando `/tests` no esté disponible; sin exigir participantes de Chat históricos. |
| [Inline Chat](https://code.visualstudio.com/docs/chat/inline-chat) | Atajos y alternativa mediante Ask in Chat. |
| [Instrucciones personalizadas](https://code.visualstudio.com/docs/agent-customization/custom-instructions) | Instrucciones de repositorio y archivo C# con `applyTo`. |
| [Microsoft.AspNetCore.OpenApi 10.0.14](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/10.0.14) | Dependencia explícita del proyecto API. |
| [Microsoft.AspNetCore.Mvc.Testing 10.0.14](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing/10.0.14) | Dependencia explícita de las pruebas. |

Se conserva xUnit 2 por la sencillez de su integración con el ejecutor de pruebas utilizado. Las versiones se fijan en los `.csproj`. Las capacidades y nombres de controles de Copilot pueden variar; los prompts expresan el resultado requerido independientemente del atajo.

## Correspondencia de los tres ejercicios

| Original | Archivo o instrucción local |
|---|---|
| Nueva ruta en `main.py` | TODO de `Program.cs`; paso 1 de `docs/GUIA.md`. |
| Prueba pytest para Spain | Nueva clase `CitiesEndpointTests.cs` generada por el participante; paso 2. |
| Documentar ejecución con un agente | Generar `docs/USO-API.md`; paso 3. |
| Inspección de API en `/docs` | Scalar con envío de solicitudes, ejemplos OpenAPI y tabla de entradas en `/ejemplos`. |
| Preparación/limpieza de Codespaces | Clonación local, `dotnet restore`, `dotnet run` y Ctrl+C. |

La referencia del instructor permanece en `docs/SOLUCION.md`. El código de partida deja pendiente la ruta del primer ejercicio.
