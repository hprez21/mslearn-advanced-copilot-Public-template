# Guía del instructor

Esta práctica adapta [mslearn-advanced-copilot](https://github.com/MicrosoftDocs/mslearn-advanced-copilot) a C# y ASP.NET Core Minimal APIs. El grupo trabaja en su equipo y tú decides cuándo iniciar, revisar y cerrar cada actividad. Las pruebas comprueban el software; no califican a los participantes ni registran el avance del curso.

## Antes de la sesión

1. Comprueba que cada participante dispone del SDK estable de .NET 10, Git, un editor con soporte para C# y acceso a GitHub Copilot en ese editor. Tener únicamente el runtime no permite compilar. En la raíz del repositorio, `dotnet --version` debe mostrar una versión `10.0.x`.
2. Abre la carpeta completa del repositorio. En VS Code, verifica que C# Dev Kit y las funciones de Copilot estén disponibles y que se haya iniciado sesión con la cuenta correcta. Los nombres y la disponibilidad de controles pueden variar por versión y por las políticas de la organización.
3. Ejecuta los comandos de preparación desde la raíz:

   ```text
   dotnet restore TravelWeather.sln
   dotnet build TravelWeather.sln --no-restore
   dotnet test TravelWeather.sln --no-build
   dotnet run --project src/TravelWeather.Api --no-launch-profile --urls http://localhost:5050
   ```

4. Abre Scalar en `http://localhost:5050/docs` y la tabla de entradas en `http://localhost:5050/ejemplos`. En la operación de clima, pulsa **Test Request**, introduce `Spain`, `Seville`, `January` en los parámetros de ruta y pulsa **Send**. Debe responder `200` con `{"high":61,"low":41}`. No se envía un cuerpo JSON.
5. Comprueba que `src/TravelWeather.Api/Program.cs` conserva el TODO para `GET /countries/{country}`. Esta entrega inicial deja esa ruta pendiente. Si reutilizas una copia ya resuelta, prepara una copia limpia de esta entrega antes de la clase.

La restauración necesita acceso a NuGet y Copilot necesita conexión al servicio. La API utiliza un archivo JSON local y no llama a un proveedor de clima. No es necesario crear un Codespace, un contenedor ni un recurso en Azure.

## Recorrido propuesto

Los tiempos son una orientación para una sesión de 45 minutos; continúa cuando el grupo haya alcanzado el objetivo.

| Etapa | Tiempo orientativo | Evidencia que puedes revisar |
|---|---:|---|
| Preparación y lectura del proyecto | 8 min | API local y pruebas iniciales ejecutándose |
| 1. Crear la ruta con Inline Chat | 12 min | Respuestas reales para Spain, Portugal y un país inexistente |
| 2. Generar y mejorar pruebas con Chat | 12 min | Pruebas HTTP que expresan el contrato y pasan localmente |
| 3. Documentar con Agent | 8 min | `docs/USO-API.md` contrastado con código y ejecución |
| Cierre y revisión | 5 min | Cambios revisados y explicación de las decisiones |

## Presentar el proyecto

Abre `Program.cs`, `Services/WeatherRepository.cs`, `Data/weather.json` y las pruebas iniciales. Explica que los endpoints reciben `WeatherRepository` mediante inyección de dependencias y que el archivo se carga una vez al iniciar la aplicación.

Separa estos contratos antes de pedir código:

| Ruta | Estado inicial | Respuesta |
|---|---|---|
| `GET /countries` | Implementada | Arreglo de siete países |
| `GET /countries/{country}` | Pendiente: objetivo de la práctica | Arreglo de ciudades del país |
| `GET /countries/{country}/{city}/{month}` | Implementada | Objeto con `high` y `low` |

Las claves del archivo distinguen mayúsculas y minúsculas. Los meses están en inglés y `Spain` contiene únicamente `Seville`. `Portugal` contiene `Lisbon` y `Porto`. Esta adaptación interpreta los valores como grados Fahrenheit y conserva los números originales; el repositorio de origen no declara expresamente la unidad. El ejercicio consulta esos datos, sin convertirlos ni inventar ciudades.

## 1. Crear la ruta con Inline Chat

1. Sitúa el cursor junto al TODO de `Program.cs`. Abre Inline Chat, normalmente con `Ctrl+I` en Windows/Linux o `Cmd+I` en macOS. Si la sesión muestra **Ask in Chat**, utiliza esa acción o abre Chat y adjunta el archivo.
2. Pide al grupo que identifique qué información le falta a una petición como «crea una ruta de ciudades». Después utiliza un mensaje concreto:

   ```text
   En Program.cs implementa GET /countries/{country} como Minimal API.
   Usa el WeatherRepository existente y su método TryGetCities; no vuelvas
   a leer el JSON ni agregues dependencias. Si el país existe, devuelve 200
   con el arreglo JSON de ciudades ordenado ordinalmente. Si no existe,
   devuelve 404 con ProblemDetails. Respeta las claves exactas y las rutas
   actuales. Añade nombre y resumen para OpenAPI. Modifica solo esta ruta.
   ```

3. Revisa el cambio antes de aceptarlo. Confirma que la respuesta es un arreglo, que no hay ciudades codificadas en el endpoint y que el caso ausente no termina en una excepción de diccionario.
4. Detén y vuelve a iniciar la API para cargar el cambio. En Scalar recarga `/docs`, abre la operación de ciudades y usa **Test Request** para introducir `Spain`, `Portugal` y `Atlantis` en `country`. Espera `200 ["Seville"]`, `200 ["Lisbon","Porto"]` y `404`, respectivamente. `spain` también debe devolver `404` por la comparación exacta de claves. El archivo `TravelWeather.Api.http` es una alternativa.
5. Abre `/openapi/v1.json` y busca `/countries/{country}`. El JSON se genera a partir de los endpoints y Scalar lo usa para mostrar operaciones y ejemplos. La ayuda de `/ejemplos` ofrece una tabla adicional con valores para introducir.

Si aparece una respuesta incorrecta, usa el resultado HTTP como contexto para la siguiente petición. Evita pedir a Copilot que cambie los datos para acomodarlos a una propuesta errónea.

## 2. Generar y mejorar pruebas con Chat

1. Abre las pruebas iniciales y selecciona o adjunta el endpoint nuevo, el repositorio y el JSON. Utiliza Chat y el comando `/tests` si está disponible; el mismo pedido en lenguaje natural permite continuar cuando no aparece ese comando.
2. Propón el mensaje:

   ```text
   Crea CitiesEndpointTests.cs en tests/TravelWeather.Api.Tests con xUnit
   y WebApplicationFactory<Program>, siguiendo las pruebas iniciales.
   Prueba GET /countries/Spain: status 200, JSON y exactamente ["Seville"].
   Prueba Portugal: exactamente ["Lisbon","Porto"] en ese orden.
   Prueba Atlantis y spain: status 404 y ProblemDetails con status 404.
   Haz solicitudes HTTP a la aplicación, sin simular WeatherRepository.
   No cambies datos ni código productivo para hacer pasar las pruebas.
   ```

3. Lee al menos una aserción antes de ejecutar. Una prueba que solo comprueba `200`, o que espera `Madrid`, no demuestra el contrato acordado. Las pruebas de integración crean su propio servidor de pruebas y no necesitan que la API esté levantada en el puerto 5050.
4. Ejecuta `dotnet test TravelWeather.sln` y revisa el resultado. Si falla, comparte con Copilot el mensaje, la respuesta observada y el comportamiento esperado. Decide si el error está en el código o en la prueba antes de aplicar cambios.
5. Añade la comprobación de `405 Method Not Allowed` al enviar `POST /countries/Spain`: esta ruta solo admite GET, como establece la guía del participante.

Estas pruebas forman parte del aprendizaje de desarrollo asistido. Tú determinas cuándo se ha revisado evidencia suficiente para pasar al siguiente bloque.

## 3. Documentar con Agent

1. Abre una sesión local de **Agent** en Copilot y permite que lea los archivos del proyecto. Adjunta los archivos si tu editor no los descubre automáticamente.
2. Utiliza este pedido:

   ```text
   Revisa el código, los proyectos, launchSettings.json y las pruebas.
   Crea docs/USO-API.md en español para alguien que ejecutará la API
   localmente con .NET 10. Incluye prerrequisitos, restore, run y test
   desde la raíz, el puerto configurado, endpoints y ejemplos reales
   de respuestas, errores 404 y claves sensibles a mayúsculas.
   Explica Scalar en /docs, los valores de /ejemplos y el contrato
   /openapi/v1.json según su implementación real. Indica que los GET
   reciben parámetros de ruta y no un cuerpo JSON.
   Conserva README.md y las guías del laboratorio. No publiques nada,
   no agregues dependencias y no cambies el código de la aplicación.
   ```

3. Revisa los archivos que propone modificar y cualquier comando antes de permitir su ejecución. Comprueba que el documento no exige Python, Uvicorn, Docker ni Codespaces.
4. Sigue un comando del documento y verifica un ejemplo de respuesta. Corrige cualquier discrepancia mediante una nueva petición con el resultado observado.
5. Pide revisar los ejemplos OpenAPI de ciudades y, si hace falta, ampliar `/ejemplos` con Spain, Portugal y el caso ausente. Conserva Scalar en `/docs`. Reinicia la aplicación, prueba los valores en Scalar y ejecuta de nuevo las pruebas.

## Cerrar la práctica

Revisa el cambio con el grupo: qué aportó el contexto, qué propuesta hubo que corregir y qué evidencia produjo ejecutar las pruebas. No hace falta que todas las respuestas de Copilot coincidan; deben cumplir el mismo contrato.

Detén la API con `Ctrl+C`. Cada participante puede guardar los archivos o crear un commit en su copia. No hay un paso de limpieza de recursos en la nube. Consulta [la solución de referencia](SOLUCION.md) al preparar la sesión o al cerrar el ejercicio, para conservar el trabajo de razonamiento del grupo.
