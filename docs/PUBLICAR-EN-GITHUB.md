# Publicar la versión inicial en GitHub

Esta entrega está preparada para alojar **el código y las guías** en un repositorio de GitHub. Cada participante ejecutará la API en su equipo con .NET. GitHub Pages sirve contenido estático y no ejecuta esta aplicación ASP.NET Core.

## 1. Preparar la copia que vas a publicar

Usa la carpeta extraída de `copilot-dotnet-lab-v0.1.0-scalar.zip`, o esta carpeta de trabajo. El ZIP contiene únicamente los archivos de la entrega: no incluye dependencias descargadas, resultados de pruebas ni binarios.

Confirma que el proyecto conserva el TODO en `src/TravelWeather.Api/Program.cs`. `GET /countries/{country}` debe quedar pendiente para el primer ejercicio. La solución está descrita por separado en `docs/SOLUCION.md`, para que el instructor pueda consultarla.

Ejecuta desde la raíz con el SDK .NET 10 instalado:

```sh
dotnet restore TravelWeather.sln
dotnet build TravelWeather.sln --no-restore
dotnet test TravelWeather.sln --no-build
```

El archivo `.gitignore` excluye `bin`, `obj`, `.tools` y `_validation`. Conserva los archivos ocultos `.github`, `.vscode`, `.gitignore`, `.gitattributes` y `.editorconfig`; contienen instrucciones y configuración del laboratorio.

## 2. Crear el repositorio remoto

1. En tu cuenta de GitHub, crea un repositorio, por ejemplo **copilot-dotnet-lab**.
2. Elige su visibilidad según cómo compartirás la clase. Deja sin seleccionar la creación automática de README, licencia y `.gitignore`, porque la entrega ya los incluye.
3. Copia la URL del repositorio nuevo. En los comandos siguientes sustituye `URL_DEL_REPOSITORIO` por esa URL.

## 3. Subir la entrega

Si la carpeta todavía no es un repositorio Git, ejecuta desde su raíz:

```sh
git init -b main
git add .
git status --short
git commit -m "Version inicial del laboratorio de Copilot con .NET"
git remote add origin URL_DEL_REPOSITORIO
git push -u origin main
```

Revisa `git status --short` antes del commit: deben aparecer fuentes y documentación, sin las carpetas excluidas. Si ya utilizas Git en esta carpeta, conserva su historial y comprueba sus remotos antes de agregar otro.

Para identificar la versión inicial después de subirla:

```sh
git tag v0.1.0
git push origin v0.1.0
```

Si tu equipo aún no tiene configurados nombre y correo para Git, usa tus datos reales según las indicaciones de Git; la entrega no establece una identidad por ti.

## 4. Compartir con el grupo

Comparte el enlace al README del repositorio y pide seguir `docs/GUIA.md`. El participante puede clonar o descargar el ZIP de GitHub. Recomienda obtener una copia limpia de **v0.1.0** cuando necesite reiniciar el laboratorio.

Puedes marcar el repositorio como **Template repository** en su configuración para facilitar copias individuales. Es opcional; la práctica funciona con clonación o descarga.

La referencia de solución será visible para quienes tengan acceso al repositorio. Indica al grupo cuándo consultarla. El instructor dirige la revisión y el cierre de los ejercicios.

No necesitas habilitar GitHub Actions para impartir la clase. Las pruebas se ejecutan localmente mediante `dotnet test`; no hay mecanismos de calificación ni registro del progreso del entrenamiento.

Antes de la siguiente edición revisa [FUENTES_CONTENIDO.md](../FUENTES_CONTENIDO.md), actualiza las dependencias si procede y vuelve a comprobar los comandos de la guía.
