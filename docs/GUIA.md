# Guía del participante

En esta práctica ampliarás una API de clima con GitHub Copilot. Crearás una ruta para consultar ciudades, generarás pruebas de integración y documentarás cómo ejecutar el proyecto localmente. El instructor indica cuándo avanzar y revisa contigo los resultados.

## 0. Preparar el entorno local

1. Instala el **SDK de .NET 10** desde [Microsoft](https://dotnet.microsoft.com/download/dotnet/10.0). Instala el SDK completo, no solamente el runtime.
2. Instala Git y VS Code. En VS Code instala **C# Dev Kit** y habilita GitHub Copilot con tu cuenta autorizada. La configuración y disponibilidad de Copilot dependen de tu cuenta y de las políticas de tu organización.
3. Obtén la URL del repositorio publicado por el instructor. En una terminal ejecuta lo siguiente sustituyendo `URL_DEL_REPOSITORIO` por esa URL:

   ```sh
   git clone URL_DEL_REPOSITORIO copilot-dotnet-lab
   cd copilot-dotnet-lab
   code .
   ```

   También puedes usar **Code > Download ZIP** en GitHub, extraerlo y abrir la carpeta con **File > Open Folder**. Si `code` no está en el PATH, abre VS Code desde su acceso directo.

4. Abre la terminal integrada en la raíz del repositorio y ejecuta:

   ```sh
   dotnet --version
   dotnet restore TravelWeather.sln
   dotnet build TravelWeather.sln --no-restore
   dotnet test TravelWeather.sln --no-build
   ```

   Espera un SDK `10.0.x`, compilación correcta y las pruebas base aprobadas. Si falla la preparación, resuélvelo con el instructor antes del primer ejercicio.

5. Inicia la API:

   ```sh
   dotnet run --project src/TravelWeather.Api --no-launch-profile --urls http://localhost:5050
   ```

6. Visita `http://localhost:5050/docs` para abrir **Scalar**, la interfaz visual de la API. `/scalar` lleva al mismo lugar. La página `http://localhost:5050/ejemplos` reúne valores para introducir. Comprueba también estas direcciones en el navegador:

   | Dirección | Resultado esperado |
   |---|---|
   | `/countries` | `["England","France","Germany","Italy","Peru","Portugal","Spain"]` |
   | `/countries/Spain/Seville/January` | `{"high":61,"low":41}` |
   | `/countries/Spain/Madrid/January` | 404 con un objeto ProblemDetails |
   | `/openapi/v1.json` | Documento con las rutas actuales |

   Agrega siempre `http://localhost:5050` antes de la ruta. En una terminal, una alternativa para consultar el clima es:

   ```sh
   curl -i http://localhost:5050/countries/Spain/Seville/January
   ```

   En Windows PowerShell, si `curl` es un alias, usa `curl.exe` o `Invoke-RestMethod`. La extensión REST Client opcional permite usar **Send Request** en `src/TravelWeather.Api/TravelWeather.Api.http`.

7. Detén la API con **Ctrl+C**. Trabaja y ejecuta todos los comandos siguientes desde la raíz del repositorio.

## Probar solicitudes visualmente con Scalar

1. Con la API iniciada, abre `http://localhost:5050/docs` y selecciona **GET `/countries/{country}/{city}/{month}`**.
2. Pulsa **Test Request** para abrir el cliente de solicitudes. En algunas versiones el control puede aparecer como **Try It**.
3. Completa los parámetros de ruta con los valores de la tabla. No incluyas las llaves `{}` ni comillas alrededor de los valores.
4. Comprueba que la dirección corresponde a tu API local y pulsa **Send**. Revisa el código de estado y el cuerpo de la respuesta.

| Caso | `country` | `city` | `month` | Resultado esperado |
|---|---|---|---|---|
| Clima de Seville | `Spain` | `Seville` | `January` | 200: `{"high":61,"low":41}` |
| Clima de London | `England` | `London` | `January` | 200: `{"high":45,"low":36}` |
| Ciudad ausente | `Spain` | `Madrid` | `January` | 404 ProblemDetails |
| Clave con otra capitalización | `spain` | `Seville` | `January` | 404 ProblemDetails |

**No se inserta un cuerpo JSON:** estas operaciones son GET y sus entradas son los parámetros de ruta. Para **GET `/countries`** no hay campos que completar. Cuando esté implementada **GET `/countries/{country}`**, introduce `Spain` para recibir `["Seville"]`, `Portugal` para recibir `["Lisbon","Porto"]` o `Atlantis` para comprobar el 404.

Los ejemplos de los parámetros y las respuestas se incluyen en el documento OpenAPI que lee Scalar. Al agregar una ruta y reiniciar la API, recarga Scalar para ver su operación actualizada.

## Conocer el proyecto y su contexto

1. Abre `src/TravelWeather.Api/Program.cs`. Localiza `MapGet`, la inyección de `WeatherRepository`, `MapOpenApi`, `MapScalarApiReference` y el comentario `TODO` de la ruta de ciudades.
2. Abre `Services/WeatherRepository.cs`, `Models/MonthlyWeather.cs` y `Data/weather.json` dentro del proyecto API. El repositorio carga el JSON una sola vez y consulta los datos en memoria.
3. Comprueba los datos: Spain contiene solo Seville; Portugal contiene Lisbon y Porto. Las claves conservan el inglés y distinguen mayúsculas y minúsculas. Los valores originales se interpretan como Fahrenheit en esta adaptación.
4. Revisa `.github/copilot-instructions.md` y `.github/instructions/csharp.instructions.md`. Estas instrucciones aportan convenciones al asistente; sigue siendo necesario revisar sus propuestas.
5. En Copilot Chat adjunta los archivos anteriores con **Add Context** o el selector de archivos. Los nombres y controles pueden variar según la versión del cliente. Envía:

   ```text
   Explica esta Minimal API y cómo usa WeatherRepository y weather.json.
   Distingue la lista de países, la lista de ciudades que falta implementar
   y la consulta mensual existente. No modifiques archivos todavía.
   ```

Revisa que la explicación coincida con el código. Dar contexto evita que Copilot invente una estructura o datos distintos.

## 1. Crear una ruta con Inline Chat

### Contrato del ejercicio

La nueva operación será **GET `/countries/{country}`**. Devuelve únicamente un arreglo JSON de nombres de ciudades, ordenado con comparación ordinal. La consulta de temperaturas seguirá en la ruta mensual existente.

| Solicitud | Estado | Cuerpo |
|---|---|---|
| `GET /countries/Spain` | 200 | `["Seville"]` |
| `GET /countries/Portugal` | 200 | `["Lisbon","Porto"]` |
| `GET /countries/Atlantis` | 404 | ProblemDetails con `status: 404` |
| `GET /countries/spain` | 404 | ProblemDetails con `status: 404` |
| `POST /countries/Spain` sin cuerpo | 405 | No se exige un cuerpo concreto |

1. En `Program.cs`, coloca el cursor en el `TODO` de la ruta de ciudades.
2. Abre Inline Chat con **Ctrl+I** en Windows/Linux o **Cmd+I** en macOS. También puedes abrirlo con el menú contextual; si la sesión muestra **Ask in Chat**, úsalo y adjunta `Program.cs` y `WeatherRepository.cs`.
3. Escribe este prompt:

   ```text
   Implementa GET /countries/{country} en esta ASP.NET Core Minimal API.
   Inyecta WeatherRepository y usa TryGetCities. Devuelve 200 con el
   arreglo JSON de ciudades ordenado; si el país no existe, devuelve 404
   con ProblemDetails. Respeta las claves exactas del JSON. Usa TypedResults
   y metadatos OpenAPI que describan 200 y 404. La ruta solo admite GET.
   Conserva las rutas existentes. No cambies los datos ni agregues dependencias.
   ```

4. Revisa el cambio propuesto antes de aceptarlo. Debe consultar el repositorio, devolver un arreglo y manejar el país ausente. Si propone Madrid o Barcelona, adjunta `weather.json` y pide corregir la propuesta.
5. Guarda y compila:

   ```sh
   dotnet build TravelWeather.sln
   ```

6. Inicia de nuevo la API con el comando del paso 0. Prueba los cuatro GET de la tabla en Scalar, el navegador o el archivo `.http`. En Scalar selecciona la operación de ciudades y cambia el parámetro `country` en **Test Request**. Para ver el estado de POST:

   ```sh
   curl -i -X POST http://localhost:5050/countries/Spain
   ```

7. Recarga `/docs` y confirma que Scalar muestra la ruta de ciudades. Abre también `/openapi/v1.json` y busca `/countries/{country}`. Comprueba que documente GET y las respuestas 200 y 404. Esta revisión refleja la aplicación que está ejecutándose; si cambias el código, reiníciala.
8. Si algo falla, comparte el resultado concreto con Copilot: solicitud, estado, cuerpo, expectativa y archivos relevantes. Revisa y prueba la corrección. Ejemplo:

   ```text
   GET /countries/Atlantis produce 500 y esperaba 404 ProblemDetails.
   Revisa la búsqueda del país y corrige el manejo de claves inexistentes.
   Conserva el resultado de Spain y las demás rutas.
   ```

9. Detén la API con **Ctrl+C** y muestra al instructor la ruta y sus resultados antes de continuar.

## 2. Generar pruebas con Copilot Chat

1. Abre `tests/TravelWeather.Api.Tests/BaselineApiTests.cs`. Observa cómo `WebApplicationFactory<Program>` inicia la aplicación para enviar solicitudes HTTP en memoria. Estas pruebas no requieren que la API esté escuchando en el puerto 5050.
2. Adjunta las pruebas base, `Program.cs` y `weather.json` en Copilot Chat. Puedes usar `/tests` si aparece en tu cliente, o enviar directamente:

   ```text
   Crea CitiesEndpointTests.cs en tests/TravelWeather.Api.Tests con xUnit
   y WebApplicationFactory<Program>, siguiendo las pruebas existentes.
   Prueba GET /countries/Spain: 200, application/json y arreglo exacto
   ["Seville"]. Prueba Portugal con ["Lisbon","Porto"]. Prueba Atlantis
   y spain: 404, application/problem+json y status 404 en el cuerpo.
   Prueba POST sin cuerpo a /countries/Spain: 405.
   Usa los datos reales del archivo. No modifiques las pruebas base.
   ```

3. Revisa que las pruebas envíen solicitudes HTTP a la aplicación real y deserialicen el JSON. Comprobar que un texto contiene `Seville` no garantiza un arreglo exacto ni excluye ciudades inventadas.
4. Guarda y ejecuta:

   ```sh
   dotnet test TravelWeather.sln
   ```

5. Cuando haya un fallo, lee primero qué compara la prueba. Distingue un defecto de la API, una expectativa errónea y un problema de compilación. Comparte esa evidencia con Copilot. Si una prueba espera Madrid, pide corregir la expectativa usando el JSON; si el país ausente produce 500, corrige la API.
6. Vuelve a ejecutar las pruebas hasta que los casos acordados pasen. Revisa que las pruebas base sigan pasando y muestra al instructor el código y el resultado.

El resultado verde aporta evidencia de los casos ejecutados. El instructor revisa también el contrato, la claridad del código y los casos que falten.

## 3. Documentar el proyecto con un agente

1. Abre Copilot Chat y selecciona el agente que pueda explorar y editar los archivos del proyecto en tu sesión local. Si esa función no está disponible, pide el borrador en Chat y aplica tú los cambios.
2. Adjunta `Program.cs`, los `.csproj`, las pruebas, `global.json` y el README, o pide al agente local que los lea. Envía:

   ```text
   Lee la API, los proyectos, global.json y las pruebas actuales.
   Crea docs/USO-API.md en español con requisitos del SDK .NET 10,
   dotnet restore, build, test y run desde la raíz, usando HTTP local
   en localhost:5050. Documenta las rutas realmente implementadas,
   ejemplos JSON, 404, mayúsculas y meses en inglés, y cómo detener la API.
   Explica que /docs es Scalar, /ejemplos contiene valores para introducir
   y /openapi/v1.json es el contrato generado. Todas las entradas de estos
   GET son parámetros de ruta: no hay cuerpo JSON.
   Incluye la nueva ruta de ciudades con Spain y Portugal.
   Conserva el README y las guías de entrenamiento. No agregues servicios
   externos, Codespaces ni despliegues. Si no ejecutas un comando,
   no afirmes que lo probaste.
   ```

3. Revisa el diff y las acciones que proponga el agente antes de aceptarlas. Comprueba que `docs/USO-API.md` describe la API que acabas de construir y que los comandos usan las rutas reales del repositorio.
4. Ejecuta desde la raíz los comandos escritos en esa guía. Detén la API antes de volver a compilar. Comprueba que otra persona pueda seguir las instrucciones.
5. Pide a Copilot revisar los ejemplos de la nueva operación en OpenAPI y ampliar, si hace falta, la ayuda de `/ejemplos` con Spain, Portugal y el caso de país ausente. Conserva Scalar en `/docs`. Revisa el cambio, reinicia la API y prueba los ejemplos en Scalar.
6. Ejecuta `dotnet test TravelWeather.sln` después de los cambios y revisa el diff final. Si clonaste con Git puedes usar:

   ```sh
   git diff --stat
   git diff
   ```

7. Presenta al instructor la ruta, las pruebas y la documentación. El instructor cierra la actividad.

## Problemas frecuentes

| Síntoma | Acción |
|---|---|
| No se encuentra un SDK compatible | Instala un SDK estable 10.0.x y vuelve a abrir la terminal; revisa `dotnet --list-sdks`. El runtime por sí solo no compila. |
| Error de restauración de paquetes | Comprueba acceso a NuGet, proxy y conexión; vuelve a ejecutar `dotnet restore TravelWeather.sln`. |
| Puerto 5050 ocupado | Detén la instancia anterior. Si necesitas otro puerto, cambia `--urls` y la dirección de las solicitudes y del navegador. |
| Error al sobrescribir una DLL al compilar | Detén el proceso de la API con Ctrl+C y repite la compilación. |
| `/countries/Spain` devuelve 404 al inicio | Es el comportamiento previsto: la ruta forma parte del primer ejercicio. |
| `/countries/spain` devuelve 404 tras implementarla | Las claves de los datos distinguen mayúsculas; usa `Spain`. |
| Scalar no muestra un cambio de código | Reinicia la API, recarga `/docs` y comprueba el documento `/openapi/v1.json`. |
| No sabes qué escribir en Scalar | Usa la tabla de `/ejemplos`: por ejemplo `Spain`, `Seville`, `January`, sin comillas ni llaves y sin cuerpo JSON. |
| Copilot o `/tests` no está disponible | Revisa autenticación y políticas con el instructor; usa el prompt equivalente en Chat si está habilitado. |

Vuelve al [README](../README.md) para consultar la estructura y los enlaces del proyecto.
