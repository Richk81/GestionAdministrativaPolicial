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

## 👨‍💻 Autor
- **"Richard" Arroyo**