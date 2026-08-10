# Troubleshooting

Problemas comunes y cómo resolverlos.

## El bridge no arranca

### `dotnet: command not found` o `.NET SDK no está instalado`

```powershell
# Verificar instalación
dotnet --version
# Debe responder 6.x

# Si no, instalar .NET 6 SDK x86
winget install Microsoft.DotNet.SDK.6 --architecture x86
```

### `MGWServicios.dll no encontrado`

El bridge no encuentra el SDK de CONTPAQi. Soluciones:

1. **Verificar que CONTPAQi está instalado**:
   ```powershell
   Test-Path "C:\Program Files (x86)\Compac\COMERCIAL\MGWServicios.dll"
   # Debe devolver True
   ```

2. **Verificar el Registro de Windows**:
   ```powershell
   Get-ItemProperty "HKLM:\SOFTWARE\WOW6432Node\Computación en Acción, SA CV\CONTPAQ I COMERCIAL" -ErrorAction SilentlyContinue
   # Debe mostrar "DIRECTORIOBASE" apuntando a la carpeta de Comercial
   ```

3. **Si está instalado en otra ruta**, edita `appsettings.json`:
   ```json
   {
     "Contpaqi": {
       "SdkDllPath": "C:\\OTRA_RUTA\\MGW_SDK.dll"
     }
   }
   ```

---

## Error 3 (CACSql.dll)

Síntoma: `fSetNombrePAQ` retorna código 3.

**Causa:** Las DLLs nativas del SDK no se cargan por PATH incorrecto.

**Solución:**

1. Verifica que el archivo `appsettings.json` tiene `"DefaultClave"` correcta del usuario SUPERVISOR.

2. Verifica que la instancia SQL Server `COMPAC22` existe:
   ```powershell
   sqlcmd -L  # Listar instancias
   # Debe aparecer MSSQLSERVER o SQLEXPRESS
   ```

3. Verifica que la base de datos de la empresa existe y el usuario `sa` tiene acceso.

4. Si el error persiste, agrega manualmente al PATH antes de iniciar:
   ```powershell
   $env:PATH = "C:\Program Files (x86)\Compac\COMERCIAL;$env:PATH"
   $env:PATH = "C:\Program Files (x86)\Compac\Servidor de Aplicaciones;$env:PATH"
   .\ContpaqiBridge.exe
   ```

---

## Fatal Error 0xC0000005 (Access Violation)

Síntoma: El bridge crashea sin error claro, especialmente al crear documentos.

**Causa:** Marshalling incorrecto de estructuras nativas.

**Mitigación:** El código ya usa **funciones de bajo nivel** (`fInsertarDocumento` + `fSetDatoDocumento`) en lugar de `fAltaDocumento(ref tDocumento)`. Si persiste:

1. Verifica que la empresa tiene el campo `CRAZONSOCIAL` con tamaño correcto.
2. Verifica que `CCODIGOCLIENTE` existe antes de `fInsertarDocumento`.
3. Reporta el issue con el log completo.

---

## 401 Unauthorized

Síntoma: Todas las peticiones devuelven 401.

**Causa:** No envías el header `X-Api-Key`.

**Solución:**

```bash
# Header (recomendado)
curl -H "X-Api-Key: tu_clave" http://localhost:5000/api/Empresas

# Query string
curl "http://localhost:5000/api/Empresas?api_key=tu_clave"
```

**Otra causa:** El bridge no tiene `Bridge:ApiKey` configurado en `appsettings.json`. Agrega:

```json
{
  "Bridge": {
    "ApiKey": "una_clave_larga_de_64_caracteres_minimo"
  }
}
```

---

## 403 Forbidden

Síntoma: La API Key es incorrecta.

**Causa:** La clave que envías no coincide con la del servidor.

**Solución:**
1. Verifica que el valor en `appsettings.json` no tiene espacios extra.
2. Si editaste el archivo mientras el bridge corre, **reinícialo** (no recarga config en caliente).
3. Verifica que no hay caracteres especiales escapados mal.

---

## Error "La carpeta de empresas no existe"

Síntoma: `GET /api/Empresas` devuelve count=0.

**Causa:** `Contpaqi:EmpresasPath` apunta a una ruta incorrecta.

**Solución:** Edita `appsettings.json`:

```json
{
  "Contpaqi": {
    "EmpresasPath": "C:\\Compac\\Empresas"  // ← verifica esta ruta
  }
}
```

Usa `\\` (doble backslash) en JSON, no `\` simple.

---

## Timbrado falla con "No se pudo conectar con el Servidor de Licencias"

Síntoma: `fInicializaLicenseInfo(1)` retorna -1.

**Causas posibles:**
1. **CONTPAQi no está activado** en esta máquina.
2. **Licencia multiusuario insuficiente** (necesitas 5+ usuarios en la licencia para que el SDK funcione remotamente).
3. **Servidor de Licencias no accesible** desde esta PC.
4. **Firewall** bloqueando el puerto de licencias.

**Solución:**
1. Abre CONTPAQi manualmente y verifica que arranca sin pedir activación.
3. Verifica que la licencia es multiusuario (contacta a tu proveedor de CONTPAQi).
4. Ejecuta el bridge **como la misma cuenta de Windows** que usa CONTPAQi normalmente.

---

## XML no se descarga

Síntoma: `GET /api/Documentos/xml` devuelve éxito pero el XML está vacío.

**Causa:** El SDK guarda el XML en una subcarpeta que el bridge no detecta.

**Solución:** El bridge ya escanea:
- `{rutaEmpresa}\XML_SDK\{serie}{folio}.xml`
- `{rutaEmpresa}\{serie}{folio}.xml`
- `AppDomain.CurrentDomain.BaseDirectory\Factura_{folio}.xml`

Si tu SDK usa otra carpeta:

1. Verifica manualmente dónde está el XML después de timbrar:
   ```powershell
   Get-ChildItem -Path "C:\Compac\Empresas\adTU_EMPRESA" -Recurse -Filter "*.xml" | Sort-Object LastWriteTime -Descending | Select-Object -First 5
   ```

2. Si está en otra ubicación, ajusta `ObtenerXml` en `ContpaqiSdkService.cs` y agrega la ruta a la lista `posiblesRutas`.

---

## Cancelación CFDI 4.0 falla

### "UUID no encontrado"

El CFDI no está timbrado o no se ha propagado al SAT. Espera unos minutos e intenta de nuevo.

### "Error 102 - El UUID no existe en el SAT"

El documento no está timbrado aún. Verifica con:
```
GET /api/Documentos/uuid?rutaEmpresa=...&codigoConcepto=4&serie=A&folio=1234
```

### "El comprobante no se puede cancelar porque ya fue cancelado"

Ya está cancelado. Verifica el acuse.

---

## Sync bidireccional lento

Síntoma: `GET /api/Sync/clientes` con muchos registros tarda minutos.

**Causa:** El método itera todos los registros sin paginación.

**Solución:**

1. Usa `modificados?desde=` en vez del snapshot completo.
2. Limita `&limite=500` y pagina:
   ```
   GET /api/Sync/clientes/modificados?desde=2024-01-01&limite=500
   ```
3. Si la base tiene >5000 clientes, considera agregar paginación al SQL.

---

## Webhooks no llegan

**Checklist:**

1. **¿Registraste el webhook?**
   ```
   GET /api/Webhooks  → debe listarlo
   ```

2. **¿Tu endpoint Laravel responde 200?** El bridge espera respuesta en <5 segundos.

3. **¿Tu endpoint es accesible desde el bridge?** Si están en redes diferentes, necesitas URL pública.

4. **¿Hay firewall?** El bridge hace POST saliente. Verifica que puede salir al puerto 443.

5. **¿URL mal escrita?** El bridge valida `Uri.TryCreate(url, UriKind.Absolute, ...)`.

**Probar manualmente:**
```bash
# Forzar emisión de webhook
curl -X POST http://localhost:5000/api/Webhooks/emit \
  -H "X-Api-Key: tu_clave" \
  -H "Content-Type: application/json" \
  -d '{"evento":"test","payload":{"hola":"mundo"}}'
```

---

## Build falla con "No se pudo restaurar" (NuGet)

Causa: Sin acceso a NuGet o credenciales.

**Solución:** El proyecto no usa NuGet packages externos (Swashbuckle fue removido precisamente por esto). Si agregas dependencias nuevas:

```powershell
# Configurar NuGet
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# O usar un proxy/mirror interno
dotnet nuget add source https://tu-proxy/v3/index.json -n proxy
```

---

## El proceso se queda colgado

**Causa:** Algún request dejó la sesión SDK en estado inconsistente.

**Solución:** Reiniciar el proceso.

```powershell
Get-Process ContpaqiBridge | Stop-Process -Force
.\start_bridge.ps1
```

**Prevención:** El bridge serializa todas las operaciones con `lock`, así que esto solo pasa si el SDK crashea internamente. Reporta el caso con el log.

---

## ¿Aún tienes problemas?

1. Revisa el log del bridge (`stdout.log`, `stderr.log` si redirigiste).
2. Habilita debug:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug",
         "Microsoft.AspNetCore": "Information"
       }
     }
   }
   ```
3. Captura el flujo completo con Fiddler o `curl -v`.
4. Reporta el issue incluyendo: versión de CONTPAQi, versión de Windows, log completo, request que falla.