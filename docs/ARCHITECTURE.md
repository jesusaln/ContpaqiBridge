# Arquitectura interna

Este documento explica cómo funciona ContpaqiBridge por dentro.

## Vista general

```
┌─────────────────────────────────────────────────────────────────┐
│              Tu aplicación (Laravel, Node, etc.)                │
└─────────────────────────────┬───────────────────────────────────┘
                              │ HTTPS/HTTP + API Key
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│   ContpaqiBridge (.NET 6 ASP.NET Core, x86)                    │
│   ┌──────────────────────────────────────────────────────────┐  │
│   │ ASP.NET Core Pipeline                                    │  │
│   │   1. CORS middleware                                    │  │
│   │   2. ApiKeyMiddleware (auth)                            │  │
│   │   3. Authorization                                       │  │
│   │   4. MapControllers                                      │  │
│   └──────────────────────────────────────────────────────────┘  │
│                              │                                  │
│                              ▼                                  │
│   ┌──────────────────────────────────────────────────────────┐  │
│   │ Controllers (thin)                                       │  │
│   │   - Validan input                                        │  │
│   │   - Llaman a IContpaqiSdkService                          │  │
│   │   - Devuelven JSON                                       │  │
│   └──────────────────────────────────────────────────────────┘  │
│                              │                                  │
│                              ▼                                  │
│   ┌──────────────────────────────────────────────────────────┐  │
│   │ ContpaqiSdkService (singleton)                           │  │
│   │   - Thread-safe (lock por operación)                     │  │
│   │   - P/Invoke a MGWServicios.dll                          │  │
│   │   - SQL queries vía sqlcmd.exe                           │  │
│   │   - Webhooks en memoria                                  │  │
│   └──────────────────────────────────────────────────────────┘  │
└──────────────┬──────────────────────────────────┬───────────────┘
               │                                  │
               ▼                                  ▼
   ┌────────────────────────┐      ┌────────────────────────────┐
   │ MGWServicios.dll       │      │ SQL Server (COMPAC22)       │
   │ (CONTPAQi SDK 32-bit)  │      │ vía sqlcmd.exe              │
   └────────────────────────┘      └────────────────────────────┘
               │                                  │
               └──────────────┬───────────────────┘
                              ▼
                  ┌──────────────────────┐
                  │ CONTPAQi Comercial   │
                  │ (SQL + archivos)     │
                  └──────────────────────┘
```

## Por qué .NET 6 x86

CONTPAQi expone su API mediante un SDK COM/DLL **32 bits**. Llamarlo desde un proceso 64 bits causa:
- `BadImageFormatException`
- Memory access violations (crashes)
- Comportamiento indefinido

Por eso:
- `<PlatformTarget>x86</PlatformTarget>` en el `.csproj`
- `dotnet run --runtime win-x86` o runtime x86 instalado
- **No funciona en Linux** (el SDK es solo Windows)

## Pipeline de inicialización

Cada request que necesita el SDK sigue este flujo:

```
1. InicializarSDK()
   ├─ Leer HKLM\SOFTWARE\WOW6432Node\Computación en Acción\CONTPAQ I COMERCIAL
   │  └─ Obtener DirectorioBase
   ├─ SetDllDirectory(DirectorioBase)
   ├─ SetCurrentDirectory(DirectorioBase)
   ├─ Agregar al PATH: Servidor de Aplicaciones, etc.
   └─ fSetNombrePAQ("CONTPAQ I Comercial") ← inicializa MGW_SDK.dll

2. AbrirEmpresa(rutaEmpresa)
   └─ fAbreEmpresa(rutaCompleta)

3. Operación (CrearCliente, CrearFactura, etc.)
   └─ Llamadas fAlta*, fInsertar*, fSetDato*, fGuarda*

4. CerrarEmpresa()
   └─ fCierraEmpresa()
```

**Singleton con lock:** El servicio es singleton y todas las operaciones usan `lock(_lock)` para serializarlas. Esto es porque el SDK de CONTPAQi **no es thread-safe** y llamadas concurrentes corrompen el estado.

## P/Invoke

Todas las funciones del SDK se importan con `DllImport`:

```csharp
[DllImport("MGWServicios.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
private static extern int fSetNombrePAQ(string aSistema);
```

**Convención de llamada**: `StdCall` (específico del SDK de CONTPAQi).

**Naming convention**: `f` + CamelCase. Ej: `fAltaCteProv`, `fSetDatoDocumento`.

**Estructuras marshalled**: `tDocumento`, `tCteProv`, `tProducto`, `tLlaveDocto` están definidas con `[StructLayout(LayoutKind.Sequential)]` y `[MarshalAs(UnmanagedType.ByValTStr, SizeConst = X)]` para strings de tamaño fijo.

**Bypass de estructuras**: Para evitar crashes `0xC0000005` por marshalling incorrecto, las operaciones complejas usan las funciones de **bajo nivel**:
- `fInsertarDocumento` + `fSetDatoDocumento(campo, valor)` + `fGuardaDocumento`
- En lugar de `fAltaDocumento(ref id, ref tDocumento)`

## Acceso SQL directo

Para operaciones de solo-lectura que el SDK no expone (catálogos, sync, reportes), usamos `sqlcmd.exe`:

```csharp
private string EjecutarSqlCmd(string instance, string user, string pass, string bd, string sqlQuery)
{
    var psi = new ProcessStartInfo {
        FileName = @"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE",
        Arguments = $"-S \"{instance}\" -U \"{user}\" -P \"{pass}\" -d \"{bd}\" -Q \"{sqlQuery}\" -W -h -1",
        ...
    };
    // Ejecutar y leer stdout
}
```

**Ruta del sqlcmd**: Hardcoded a `170\Tools\Binn`. Si tienes otra versión de SQL Server, edita esta ruta.

**Por qué sqlcmd en vez de SqlClient?**:
- El proyecto es x86; usar `Microsoft.Data.SqlClient` desde x86 a veces da problemas con instancias de SQL Server 2019+ (64-bit).
- `sqlcmd.exe` viene con SQL Server y es independiente de la arquitectura.
- Permite conexión nativa sin drivers adicionales.

## Webhooks

Los webhooks se almacenan **en memoria** (lista estática con lock).

```csharp
private readonly List<(string evento, string url)> _webhooks = new();
```

**Limitaciones:**
- Se pierden al reiniciar el bridge.
- No tienen autenticación (añade un secret en la URL si necesitas).
- Se disparan en background (`Task.Run`) para no bloquear la operación principal.
- Timeout de 5 segundos por webhook.

**Mejora futura:** Persistir webhooks en `webhooks.json` y recargarlos al iniciar.

## Threading

- **Singleton** del servicio (una sola conexión al SDK por proceso).
- **`lock(_lock)`** en TODAS las operaciones que tocan el SDK.
- **`lock(_webhooksLock)`** para la lista de webhooks.
- Webhooks disparan `Task.Run` para no bloquear.

## Manejo de errores

Cada operación del SDK devuelve un código numérico (`int`). `0` = éxito, otros = error.

```csharp
int result = fAltaCteProv(ref idCliente, ref cliente);
if (result != 0) {
    string err = GetUltimoError(result);
    return (false, $"Error: {err}", 0);
}
```

**`GetUltimoError(int code)`** llama a `fError(code, buffer, len)` que rellena el buffer con el mensaje legible en español.

## Archivos clave

| Archivo | Líneas | Responsabilidad |
|---|---|---|
| `Program.cs` | ~40 | Pipeline ASP.NET Core |
| `Services/ContpaqiSdkService.cs` | ~3300 | Toda la lógica del SDK + SQL + sync + webhooks |
| `Services/IContpaqiSdkService.cs` | ~50 | Interfaz pública del servicio |
| `Middleware/ApiKeyMiddleware.cs` | ~85 | Validación de API Key |
| `Controllers/*.cs` | varía | Endpoints REST |

## Limitaciones conocidas

1. **x86 only**: No se puede ejecutar en x64 ni en Linux.
2. **SQL Server**: Requiere `sqlcmd.exe` instalado (viene con SQL Server Management Studio).
3. **Una empresa a la vez**: El SDK maneja una empresa abierta simultáneamente. El bridge serializa requests pero si abres desde otra app nativamente, hay conflicto.
4. **Webhooks en memoria**: Se pierden al reiniciar.
5. **Sin HTTPS**: El bridge expone HTTP. Usa reverse proxy (IIS/Caddy/nginx) para TLS.
6. **CORS abierto**: Por defecto `AllowAnyOrigin`. Cambiar en producción.