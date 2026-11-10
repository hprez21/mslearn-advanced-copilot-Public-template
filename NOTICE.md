# Procedencia, atribución y cambios

Esta entrega adapta **Apply advanced GitHub Copilot techniques**, de Microsoft y los colaboradores de [MicrosoftDocs/mslearn-advanced-copilot](https://github.com/MicrosoftDocs/mslearn-advanced-copilot).

- Referencia consultada: commit [`978400834b44ec00ef2e2bf89b5013186939dfaf`](https://github.com/MicrosoftDocs/mslearn-advanced-copilot/tree/978400834b44ec00ef2e2bf89b5013186939dfaf).
- Fecha de consulta de la adaptación: 9 de noviembre de 2026.
- Documentación original y adaptación: [Creative Commons Attribution 4.0 International](LICENSE).
- Código del repositorio original y de esta adaptación: [MIT](LICENSE-CODE). Se conserva el aviso de copyright de Microsoft.

## Cambios de la adaptación

- Python y FastAPI se sustituyen por C# y ASP.NET Core Minimal APIs sobre .NET 10.
- Las pruebas pytest se sustituyen por pruebas HTTP de integración con xUnit y `WebApplicationFactory`.
- La preparación con Codespaces se sustituye por herramientas instaladas en el equipo y comandos de .NET CLI.
- Se conserva el conjunto de datos de `weather.json`, con los mismos países, ciudades, meses y valores. Se interpreta Fahrenheit como decisión de esta adaptación; el archivo original no declara expresamente la unidad.
- Se mantienen los tres ejercicios centrales: ruta de ciudades, pruebas y documentación con Copilot. Se añaden contratos precisos, manejo de ausentes y guías de participante e instructor.
- La documentación creada durante la práctica se guarda en `docs/USO-API.md` para conservar el README que contiene las instrucciones del entrenamiento.
- `/docs` ofrece Scalar para explorar y enviar solicitudes a la API local; `/scalar` redirige a esa interfaz. `/ejemplos` reúne valores para introducir y `/openapi/v1.json` contiene el contrato generado con ejemplos.
- Se añaden instrucciones de C#, solicitudes HTTP locales y preparación para publicar el repositorio.

El código C# es una reimplementación para esta práctica. No se incluyen el código Python, las imágenes, la configuración de Codespaces ni los contenedores del original.

Esta adaptación no representa una publicación oficial de Microsoft o GitHub y no otorga una insignia ni una validación oficial de finalización. El uso de nombres de productos identifica las herramientas y fuentes del ejercicio; sus marcas pertenecen a sus titulares.
