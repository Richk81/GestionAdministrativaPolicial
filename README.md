# 🚓 Gestión Administrativa Policial – Proyecto 2025

Aplicación desarrollada en C# y .NET 8.0 para la gestión de personal policial, chalecos antibalas, armas, vehículos y usuarios administrativos. Implementa arquitectura por capas, acceso a base de datos con Entity Framework y almacenamiento en Firebase.

---

## ⚙️ Tecnologías utilizadas

- .NET 8.0
- ASP.NET Core MVC
- Entity Framework Core
- Firebase Authentication & Storage
- SQL Server
- Arquitectura en capas (Entity, DAL, BLL, IOC, Web)

---

## 📁 Estructura del proyecto

- `GestionAdminPolicial.Entity` – Entidades del dominio
- `GestionAdminPolicial.DAL` – Repositorios y contexto EF
- `GestionAdminPolicial.BLL` – Lógica de negocio
- `GestionAdminPolicial.IOC` – Inyección de dependencias
- `GestionAdminPolicial.AplicacionWeb` – Aplicación ASP.NET Core

---

## 📌 Dependencias críticas

| Paquete                     | Versión fijada | Motivo                                                   |
|----------------------------|----------------|----------------------------------------------------------|
| FirebaseAuthentication.net | `3.1.0`         | Compatible con `FirebaseAuthProvider` y `FirebaseConfig`. **NO actualizar a 4.x** sin pruebas. |
| FirebaseStorage.net        | `1.0.3`         | Compatible con Firebase Storage y .NET 8.0               |

---

## 🚨 Incidente técnico documentado

### ❗ Error tras limpiar la solución

Después de usar `Compilar > Limpiar solución`, la app no iniciaba con el error:

```
El sistema no puede encontrar el archivo especificado
```

### 🧩 Causa

El proyecto `GestionAdminPolicial.BLL` no compilaba porque la versión `4.1.0` del paquete `FirebaseAuthentication.net` no incluye las clases `FirebaseAuthProvider` ni `FirebaseConfig`. Esto rompía la compilación de toda la solución, ya que otros proyectos dependen de ese.

### ✅ Solución

- Se volvió a la versión **3.1.0** del paquete
- Se ejecutó `dotnet restore`
- Se recompiló la solución y todo volvió a funcionar correctamente

---

## 🚀 Despliegue del proyecto

### 🖥️ Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 o superior
- SQL Server (Express, Developer o superior)
- Cuenta Firebase activa y configurada
- Acceso a internet

---

### 🔧 Pasos de despliegue

1. **Clonar o copiar el proyecto**
2. **Configurar cadena de conexión**
   En `appsettings.json` del proyecto web:
   ```json
   "ConnectionStrings": {
     "CadenaSQL": "Server=TU_SERVIDOR;Database=GestionOfPolicial;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```
3. **Verificar tabla `Configuracion`** en la base de datos con claves de Firebase:
   - `Recurso`: `FireBase_Storage`
   - `Propiedad`: `api_key`, `email`, `clave`, `ruta`, etc.
4. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```
5. **Compilar y ejecutar**
   - Visual Studio: `Ctrl + Shift + B` → `F5`
   - Terminal:
     ```bash
     dotnet build
     dotnet run --project GestionAdminPolicial.AplicacionWeb
     ```

---

### 🌐 Acceso a la aplicación

Una vez iniciada, la aplicación se abre en:

```
https://localhost:<puerto>
```

Ejemplo: `https://localhost:7092`

---

### 🧱 Migraciones con EF Core

Si agregás entidades nuevas:

```bash
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

---

### 📦 Publicación

1. Clic derecho en `GestionAdminPolicial.AplicacionWeb` → `Publicar`
2. Elegí carpeta local, IIS, FTP o Azure
3. Seguí el asistente para generar archivos de despliegue

---


---


---

### 💡 Nota sobre caché de JavaScript ⚠️
Durante el desarrollo, si realizás cambios en los archivos `.js` (por ejemplo `PersonalPolicial_Index.js`) y no se reflejan en el navegador, **Chrome puede estar usando una versión en caché**.

> **IMPORTANTE:** Para forzar la descarga del archivo actualizado y asegurarte de que todos los elementos de la interfaz se vean correctamente (como los botones en las tablas DataTables):
> - **Windows / Linux:** `Ctrl + Shift + R` o `Ctrl + F5`
> - **Mac:** `Cmd + Shift + R`

Esto evita que falten elementos y asegura que siempre se cargue la última versión de tus scripts.


### 🧰 Buenas prácticas de desarrollo
- Cada cambio en archivos JS/CSS: **usar Ctrl + F5** para evitar problemas de caché.
- Mantener actualizado `asp-append-version="true"` en los scripts y estilos principales.
- Documentar cambios en tablas o procedimientos almacenados.
- Verificar siempre la columna `responsivePriority` en DataTables para botones críticos.


### 📘 Documentación y Versionado de APIs – Paso a paso
Implementación realizada para documentar y versionar las APIs del proyecto GestionAdminPolicial utilizando Swagger y Asp.Versioning.

> 🧩 Paso 1: Instalar los paquetes necesarios
En la capa web (GestionAdminPolicial.AplicacionWeb), instalar los siguientes paquetes NuGet:

bash
dotnet add package Asp.Versioning.Mvc
dotnet add package Asp.Versioning.Mvc.ApiExplorer
dotnet add package Swashbuckle.AspNetCore
Estos paquetes permiten definir versiones de API y generar documentación dinámica con Swagger UI.

> ⚙️ Paso 2: Agregar los using en Program.cs
Al inicio del archivo Program.cs, agregar:

csharp
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using System.Reflection;

> 🧱 Paso 3: Configurar servicios de versionado de API
Dentro de builder.Services, después de AddControllers(), agregar:

csharp
builder.Services.AddControllers();

// ✅ Versionado de API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
🔹 Explicación:

DefaultApiVersion = new ApiVersion(1, 0) → establece la versión inicial.

AssumeDefaultVersionWhenUnspecified = true → usa la versión por defecto si no se especifica.

ReportApiVersions = true → muestra versiones disponibles en los encabezados de respuesta.

AddApiExplorer → permite que Swagger detecte y agrupe las versiones.

> 📜 Paso 4: Configurar Swagger con soporte para versionado
Agregar después del bloque anterior:

csharp
builder.Services.AddSwaggerGen(options =>
{
    var provider = builder.Services.BuildServiceProvider()
                                   .GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = $"GestionAdminPolicial API {description.ApiVersion}",
            Version = description.ApiVersion.ToString(),
            Description = "Documentación de la API versionada",
        });
    }

    // ✅ Comentarios XML
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});
💡 Esto genera automáticamente una pestaña Swagger por versión (v1, v2, etc.) y carga los comentarios <summary> y <remarks> desde el código XML.

> 🌐 Paso 5: Configurar el uso de Swagger en el pipeline
Después de app.Build(), agregar:

csharp
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                $"API {description.GroupName.ToUpperInvariant()}");
    }
});

app.MapControllers();
🔹 Esto habilita la interfaz Swagger UI con selector de versión (por ejemplo: v1, v2).

> 🧾 Paso 6: Activar generación de comentarios XML
En tu proyecto web:

Abrí las propiedades del proyecto (clic derecho → Propiedades → Compilación).

Activá la opción “Archivo de documentación XML”.

Visual Studio generará un archivo como:

Código
GestionAdminPolicial.AplicacionWeb.xml
Dentro de la carpeta bin\Debug\net8.0\.

🔹 Esto permite que Swagger lea los comentarios de los controladores y endpoints.

> 📘 Paso 7: Documentar los controladores
En tus controladores API, agregá documentación XML con etiquetas estándar:

csharp
/// <summary>
/// Obtiene los datos completos de un personal policial para edición.
/// </summary>
/// <remarks>
/// Este endpoint se utiliza para cargar el formulario de edición de un personal policial,
/// devolviendo todos los datos relacionados, como armas y domicilios.
/// </remarks>
/// <param name="id">ID único del personal policial.</param>
/// <returns>Objeto <see cref="VMPersonalPolicial"/> con los datos completos.</returns>
/// <response code="200">Datos obtenidos correctamente.</response>
/// <response code="404">No se encontró el personal con el ID especificado.</response>
/// <response code="500">Error interno del servidor.</response>
[HttpGet("ObtenerPersonalParaEditar/{id}")]
[ProducesResponseType(typeof(VMPersonalPolicial), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> ObtenerPersonalParaEditar(int id)
{
    // Lógica del endpoint
}
🧠 Consejo: mantener consistencia en la estructura de <summary>, <remarks> y <response> mejora la legibilidad del Swagger UI.

> ⚡ Paso 8: Ejecutar y probar
Ejecutá la aplicación desde Visual Studio o terminal:

bash
dotnet run --project GestionAdminPolicial.AplicacionWeb
Accedé al explorador Swagger:

Código
https://localhost:<puerto>/swagger
Verás una interfaz con versiones disponibles:

v1 → primera versión estable

v2 → versiones futuras o endpoints extendidos

---
---

## 👨‍💻 Autor by:
- **"Juan José Richard" Arroyo**