# ContpaqiBridge 🚀

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![.NET 6](https://img.shields.io/badge/.NET-6.0-512BD4)
![Node.js 20+](https://img.shields.io/badge/Node.js-20+-339933)
![MCP Compatible](https://img.shields.io/badge/MCP-compatible-blue)

**API REST auto-hospedada** que conecta sistemas externos (Laravel en VPS, Hostinger, WebApps, aplicaciones desktop) con el **SDK de CONTPAQi Comercial Premium** instalado localmente. Permite facturar, crear clientes/productos y extraer XMLs/CFDI de manera remota a través de internet.

> **Equivalente funcional a "AR Software - CONTPAQi Comercial API" pero auto-hospedado, sin dependencia de terceros y con 30+ funcionalidades** (incluye webhooks, sync bidireccional y servidor MCP).

## 🧩 ¿Qué incluye este repo?

| Carpeta | Contenido |
|---|---|
| `Program.cs`, `Services/`, `Controllers/`, `Models/`, `Middleware/` | El bridge en sí (.NET 6 ASP.NET Core, x86) |
| `docs/` | Documentación completa: [API.md](./docs/API.md), [ARCHITECTURE.md](./docs/ARCHITECTURE.md), [DEPLOYMENT.md](./docs/DEPLOYMENT.md), [TROUBLESHOOTING.md](./docs/TROUBLESHOOTING.md) |
| `Laravel-Examples/` | Cliente HTTP Laravel, comando de sync, controller de webhooks, migraciones |
| `mcp-server/` | **Servidor MCP** para usar el bridge desde opencode/Claude |
| `opencode.json` | Configuración para registrar el MCP server en opencode |

## 🤖 MCP Server (usar con opencode / Claude)

El proyecto incluye un servidor MCP (Model Context Protocol) que expone las 26 funcionalidades del bridge como herramientas para asistentes IA. Una vez configurado, puedes pedirle a Claude cosas como:

> *"Crea el cliente CLI-001 con RFC XAXX010101000 y timbra una factura por $1500 al concepto 4"*

Detalles en [mcp-server/README.md](./mcp-server/README.md).

## 🆚 Comparación con servicios comerciales

| Característica | AR Software (pago) | ContpaqiBridge |
|---|---|---|
| Costo anual por RFC | $720 USD | **$0** (self-hosted) |
| Hosting | Servidores de ellos | **Tú controlas** |
| Código fuente | No disponible | **100% abierto** |
| Webhooks | Limitado | **Sí, completos** |
| Sync bidireccional | No documentado | **Sí, con Laravel/Node/Python** |
| Servidor MCP (Claude/IA) | No | **Incluido** |
| Latencia | Internet → AR → CONTPAQi | **Directo (red local)** |
| Vendor lock-in | Sí | **No** |
| Personalización | No | **Total** |

## 🛠️ Instalación y Puesta en Marcha

### 1. Requisitos Previos
- Windows 10/11 o Windows Server 2019+ con CONTPAQi Comercial Premium instalado.
- **.NET 6 SDK x86** instalado.
- SQL Server con la base de datos de la empresa CONTPAQi.
- (Opcional) ZeroTier / Tailscale / VPN si vas a consumirlo desde fuera de tu red local.

### 2. Configuración

Edita `appsettings.json`:

```json
{
  "Contpaqi": {
    "EmpresasPath": "C:\\Compac\\Empresas",
    "DefaultUsuario": "SUPERVISOR",
    "DefaultClave": "tu_password_admin",
    "InstanceSql": "localhost\\COMPAC22",
    "SqlUser": "sa",
    "SqlPassword": "tu_password_sql"
  },
  "Bridge": {
    "ApiKey": "GENERA_UNA_CLAVE_SECRETA_LARGA_DE_64_CHARS_MIN"
  }
}
```

> ⚠️ **Cambia el `ApiKey` por una clave secreta larga y única.** Cualquier petición sin esta clave será rechazada con HTTP 401/403.

### 3. Abrir puerto 5000 en Firewall (PowerShell Admin)
```powershell
New-NetFirewallRule -DisplayName "ContpaqiBridge" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

### 4. Ejecutar
```powershell
./start_bridge.ps1
```
o manualmente:
```powershell
dotnet run --urls "http://0.0.0.0:5000"
```

### 5. Verificar
Abre `http://localhost:5000/api/Docs` → verás la documentación interactiva HTML con todos los endpoints.

Abre `http://localhost:5000/api/Status/health` → debe responder `Healthy`.

---

## 🔐 Autenticación

Todas las peticiones (excepto las públicas marcadas con 🔓) requieren la API Key:

```bash
# Vía header
curl -H "X-Api-Key: TU_CLAVE" http://localhost:5000/api/Empresas

# Vía query string
curl "http://localhost:5000/api/Empresas?api_key=TU_CLAVE"
```

**Endpoints públicos (sin auth):**
- `GET /api/Status/health` — health check
- `GET /api/Docs` — documentación HTML interactiva
- `GET /api/Docs/openapi.json` — manifiesto OpenAPI 3.0

---

## 📚 Endpoints

### 🏢 Empresas y Diagnóstico

#### 🔓 `GET /api/Status/health`
Health check del servicio.

#### 🔓 `GET /api/Docs`
Documentación HTML interactiva con todos los endpoints.

#### 🔓 `GET /api/Docs/openapi.json`
Manifiesto OpenAPI 3.0 (importable a Postman/Insomnia).

#### `GET /api/Empresas`
Lista todas las empresas disponibles en `Contpaqi:EmpresasPath`.
```json
{
  "success": true,
  "empresasPath": "C:\\Compac\\Empresas",
  "count": 3,
  "empresas": [
    { "nombre": "adMI_EMPRESA", "rutaEmpresa": "C:\\Compac\\Empresas\\adMI_EMPRESA", "valida": true, "tieneCSD": true }
  ]
}
```

#### `GET /api/Status`
Inicializa el SDK y devuelve el estado. Útil como `ping` desde tus apps.

#### `POST /api/Status/connect`
Prueba que se puede abrir una empresa.
```json
{ "companyPath": "C:\\Compac\\Empresas\\adMI_EMPRESA" }
```

#### `GET /api/Status/conceptos?rutaEmpresa=...`
Lista los conceptos de documentos (factura, nota de crédito, etc.) leyendo desde SQL.

#### `GET /api/Status/unidades?rutaEmpresa=...`
Lista el catálogo de unidades de medida del SDK.

#### `GET /api/Status/documentos?rutaEmpresa=...`
Lista los últimos documentos de la empresa.

#### `GET /api/Diagnostico/unidades?rutaEmpresa=...`
Alias de `/api/Status/unidades` para diagnóstico.

---

### 👥 Clientes

#### `POST /api/Clientes`
Crea o actualiza un cliente en CONTPAQi.
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adMI_EMPRESA",
  "codigo": "CLI001",
  "razonSocial": "Juan Pérez SA de CV",
  "rfc": "JPE950101ABC",
  "email": "contacto@ejemplo.com",
  "calle": "Av Reforma 123",
  "colonia": "Centro",
  "codigoPostal": "06000",
  "ciudad": "CDMX",
  "estado": "CDMX",
  "pais": "México",
  "regimenFiscal": "601",
  "usoCFDI": "G03",
  "formaPago": "99"
}
```

---

### 📦 Productos

#### `GET /api/Productos?rutaEmpresa=...&limite=20`
Lista los primeros N productos del catálogo.

#### `POST /api/Productos`
Crea o actualiza un producto/servicio.
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adMI_EMPRESA",
  "codigo": "PROD001",
  "nombre": "Licencia mensual",
  "descripcion": "Servicio de suscripción",
  "precio": 1500.00,
  "tipoProducto": 3,
  "unidadMedida": "E48",
  "claveSAT": "81112101"
}
```

---

### 📄 Documentos (Facturas)

#### `POST /api/Documentos/factura`
Crea una factura (sin timbrar).
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adMI_EMPRESA",
  "codigoConcepto": "4",
  "codigoCliente": "CLI001",
  "clienteRazonSocial": "Juan Pérez SA de CV",
  "clienteRFC": "JPE950101ABC",
  "clienteRegimenFiscal": "601",
  "usoCFDI": "G03",
  "formaPago": "99",
  "metodoPago": "PUE",
  "productos": [
    {
      "codigo": "PROD001",
      "nombre": "Licencia mensual",
      "cantidad": 1,
      "precio": 1500,
      "unidadMedida": "E48",
      "claveSAT": "81112101"
    }
  ]
}
```
**Respuesta:**
```json
{ "success": true, "message": "Factura creada exitosamente. Serie: A, Folio: 1234", "idDocumento": 5678 }
```

#### `POST /api/Documentos/timbrar`
Timbra una factura existente ante el SAT.
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adMI_EMPRESA",
  "codigoConcepto": "4",
  "serie": "A",
  "folio": 1234,
  "passCSD": "contraseña_del_certificado"
}
```

#### `POST /api/Documentos/validar`
Valida una factura antes de timbrar (NO envía al SAT). Detecta:
- Falta de CSD o contraseña
- Cliente sin régimen fiscal / código postal
- Productos sin Clave SAT
- Documento ya cancelado
```json
{
  "rutaEmpresa": "...",
  "codigoConcepto": "4",
  "serie": "A",
  "folio": 1234,
  "passCSD": "..."
}
```
**Respuesta:**
```json
{
  "success": true,
  "listoParaTimbrar": false,
  "errores": 2,
  "warnings": 1,
  "issues": [
    { "severidad": "Error", "categoria": "Producto", "mensaje": "Producto 'X' no tiene Clave SAT..." }
  ]
}
```

#### `GET /api/Documentos/xml?rutaEmpresa=...&codigoConcepto=4&serie=A&folio=1234`
Recupera el XML CFDI timbrado.

#### `GET /api/Documentos/ultimos?rutaEmpresa=...&cantidad=10`
Lista los últimos N documentos (diagnóstico).

#### `POST /api/Documentos/cancelar`
Cancela una factura timbrada ante el SAT (CFDI 4.0).

| Motivo | Significado |
|:--|:--|
| `01` | Con errores CON relación (requiere `uuidSustitucion`) |
| `02` | Con errores SIN relación |
| `03` | No se llevó a cabo la operación |
| `04` | Operación en factura global |
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adMI_EMPRESA",
  "codigoConcepto": "4",
  "serie": "A",
  "folio": 1234,
  "motivoCancelacion": "02",
  "passCSD": "...",
  "uuidSustitucion": ""
}
```

#### `POST /api/Documentos/cancelar-admin`
Cancela una factura solo en CONTPAQi (no afecta al SAT). Útil para errores administrativos internos.

---

### 🔄 Flujo Completo (Endpoint Estrella)

#### `POST /api/Integracion/flujo-completo`
Crea cliente + producto + factura + timbra en **una sola llamada**.

```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adMI_EMPRESA",
  "cliente": {
    "codigo": "CLI001",
    "razonSocial": "Juan Pérez",
    "rfc": "JPE950101ABC",
    "regimenFiscal": "601",
    "usoCFDI": "G03"
  },
  "producto": {
    "codigo": "PROD001",
    "nombre": "Servicio de consultoria",
    "precio": 1500.00,
    "claveSAT": "81112101",
    "unidadMedida": "E48"
  },
  "factura": {
    "codigoConcepto": "4",
    "cantidad": 1,
    "passCSD": "...",
    "usoCFDI": "G03",
    "formaPago": "99",
    "metodoPago": "PUE"
  }
}
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Proceso completado con éxito. | Factura TIMBRADA exitosamente.",
  "detalles": {
    "cliente": "CREADO",
    "producto": "CREADO",
    "factura": "CREADA",
    "timbrado": "EXITOSO"
  },
  "ids": {
    "idCliente": 123,
    "idProducto": 456,
    "idDocumento": 789,
    "serie": "A",
    "folio": 1234
  }
}
```

---

## 💻 Ejemplos de Consumo

### Laravel / PHP
```php
use Illuminate\Support\Facades\Http;

$response = Http::withHeaders(['X-Api-Key' => config('contpaqi.api_key')])
    ->post('http://192.168.191.226:5000/api/Integracion/flujo-completo', [
        'rutaEmpresa' => 'C:\\Compac\\Empresas\\adMI_EMPRESA',
        'cliente' => [
            'codigo' => 'CLI001',
            'razonSocial' => $cliente->nombre,
            'rfc' => $cliente->rfc,
            'regimenFiscal' => '601',
        ],
        'producto' => [
            'codigo' => 'PROD001',
            'nombre' => 'Servicio',
            'precio' => 1000,
            'claveSAT' => '81112101',
        ],
        'factura' => [
            'codigoConcepto' => '4',
            'cantidad' => 1,
            'passCSD' => config('contpaqi.csd_password'),
        ],
    ]);

if ($response->successful() && $response->json('success')) {
    return response()->json([
        'ok' => true,
        'folio' => $response->json('ids.folio'),
    ]);
}
```

### Node.js
```javascript
const axios = require('axios');

await axios.post('http://192.168.191.226:5000/api/Integracion/flujo-completo', payload, {
    headers: { 'X-Api-Key': process.env.CONTPAQI_API_KEY }
});
```

### Python
```python
import requests
requests.post(
    'http://192.168.191.226:5000/api/Integracion/flujo-completo',
    json=payload,
    headers={'X-Api-Key': 'tu_clave'}
)
```

---

## 🌐 Acceso Remoto desde la Nube

Tienes 3 opciones según tu presupuesto:

### Opción A — Túnel VPN (GRATIS)
- Instala **ZeroTier** o **Tailscale** en la máquina Windows y en tu VPS.
- Tu Laravel apunta a la IP del túnel (ej: `192.168.191.226:5000`).

### Opción B — Windows VPS ($300-500 MXN/mes)
- Contrata un Windows Server VPS (IONOS, Contabo, Hetzner, etc.).
- Instala CONTPAQi + SQL + .NET + este bridge.
- Abre el puerto 5000 y dale IP pública.

### Opción C — Ngrok / Cloudflare Tunnel (GRATIS limitado)
- `ngrok http 5000` te da una URL pública temporal.
- Cloudflare Tunnel es gratis y permanente con tu dominio.

---

## 🔧 Solución de Problemas

- **Error 3 (CACSql.dll)** → Verifica que el bridge corra en **x86** (ya está configurado en el `.csproj`). Si persiste, verifica que el PATH incluya la carpeta `C:\Program Files (x86)\Compac\COMERCIAL`.
- **Fatal Error 0xC0000005** → Suele pasar por marshalling incorrecto. El bridge ya usa funciones de bajo nivel para evitarlo.
- **Archivo XML no encontrado** → El SDK guarda el XML en `XML_SDK/` dentro de la empresa. El bridge ya escanea esa ruta.
- **API Key inválida** → Verifica que `Bridge:ApiKey` esté configurado y que envíes el header `X-Api-Key`.
- **Puerto 5000 ocupado** → Cambia el puerto en `Program.cs` y abre el nuevo en firewall.

---

## 📜 Licencia

Este proyecto es de tu propiedad. Cópialo, modifícalo, véndelo, distribúyelo. **Sin pago por RFC**.

---

*Desarrollado para integración Laravel-Contpaqi.*