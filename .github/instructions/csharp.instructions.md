---
description: 'Convenciones de C# y ASP.NET Core para el laboratorio'
applyTo: '**/*.cs'
---

- Utiliza .NET 10, tipos anulables y las importaciones implícitas del proyecto.
- Usa PascalCase en tipos y métodos, camelCase en variables y cuatro espacios de sangría.
- Prefiere `TypedResults` y uniones `Results<...>` cuando haya varios resultados HTTP; conserva metadatos OpenAPI coherentes.
- Usa `TryGetValue` o los métodos de `WeatherRepository` para claves proporcionadas por el usuario.
- Usa tipos de dominio para el clima y `System.Text.Json` para el JSON. No devuelvas texto serializado manualmente como si fuera un arreglo.
- Cubre casos reales y ausentes con pruebas HTTP. No simules `WeatherRepository` en pruebas que verifican su integración con la API.
- Comenta las decisiones que lo requieran; evita comentarios que repitan cada instrucción.
