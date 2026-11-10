# Comprobación de la entrega 0.1.0

Fecha: 9 de noviembre de 2026.

La entrega se comprobó en Windows con **SDK .NET 10.0.403** y **runtime ASP.NET Core 10.0.14**, usando un SDK local para la verificación. El repositorio requiere un SDK estable 10.0.x y fija sus dependencias en los archivos de proyecto.

| Comprobación | Resultado |
|---|---|
| Restauración con auditoría NuGet activa | Correcta, sin advertencias de vulnerabilidades notificadas en esa ejecución |
| Compilación de `TravelWeather.sln` | Correcta: 0 errores y 0 advertencias |
| Pruebas del proyecto inicial con Scalar | **13 aprobadas**, 0 fallidas, 0 omitidas |
| Código de `docs/SOLUCION.md` aplicado en una copia separada | **19 aprobadas**: 13 iniciales y 6 de la solución |
| Publicación Release de la API | Correcta; incluye `Data/weather.json` |
| Inicio de la DLL publicada desde otra carpeta | Correcto, usando HTTP en localhost:5050 |
| `GET /countries` | 200 con los siete países y el orden documentado |
| `GET /countries/Spain/Seville/January` | 200 con `high: 61`, `low: 41` |
| `GET /countries/Spain/Madrid/January` | 404 con ProblemDetails |
| `GET /docs` | Redirección a `/docs/`, que sirve Scalar con sus recursos JavaScript locales |
| Interfaz de Scalar en Chrome | Operaciones visibles y parámetros Spain, Seville, January precargados |
| Envío de una consulta desde Scalar | 200 real con `high: 61`, `low: 41` |
| `GET /ejemplos` | Ayuda con valores de entrada y resultados esperados |
| `GET /openapi/v1.json` | 200 con el contrato generado |
| `GET /countries/Spain` en el proyecto inicial | 404; la ruta permanece pendiente para la clase |

Las pruebas iniciales cubren raíz, países, clima mensual, claves inexistentes o con diferencias de mayúsculas, Scalar, sus recursos locales y los ejemplos de parámetros y respuestas en OpenAPI. Las pruebas de la solución añaden Spain, Portugal, países ausentes, POST no permitido y metadatos de la ruta nueva.

La solución de referencia se ejecutó en una copia aislada: no se añadió el endpoint al código inicial. El acceso a Copilot se comprueba durante la preparación de cada sesión con la cuenta y el editor del participante.

Esta copia se preparó a partir del ZIP original de la versión 0.1.0. Conserva el SDK estable 10 y el TODO del ejercicio. La integración visual utiliza `Scalar.AspNetCore` 2.19.0.

El ZIP de entrega excluye SDK, paquetes, binarios, resultados de pruebas y archivos temporales. Su contenido está preparado para subirlo a un repositorio; la publicación remota se realiza siguiendo [PUBLICAR-EN-GITHUB.md](PUBLICAR-EN-GITHUB.md).
