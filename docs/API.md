# API Reference

Documentación completa de todos los endpoints REST del ContpaqiBridge.

## Convenciones

- **Base URL**: `http://localhost:5000` (default) o tu IP/puerto configurado.
- **Autenticación**: Header `X-Api-Key: <clave>` o query `?api_key=<clave>`.
- **Content-Type**: `application/json` para POST.
- **Respuesta exitosa**: `200 OK` con JSON.
- **Errores**:
  - `400` - Datos inválidos (falta campo, formato incorrecto)
  - `401` - Falta API Key
  - `403` - API Key inválida
  - `500` - Error interno del SDK de CONTPAQi
- **Empresa CONTPAQi**: Se identifica por `rutaEmpresa` (path completo a `C:\Compac\Empresas\<empresa>`).

## Índice

- [Status y Diagnóstico](#status-y-diagnóstico)
- [Empresas](#empresas)
- [Clientes](#clientes)
- [Productos](#productos)
- [Documentos (Facturas)](#documentos-facturas)
- [Integración (flujo completo)](#integración-flujo-completo)
- [Sincronización bidireccional](#sincronización-bidireccional)
- [Reportes](#reportes)
- [Webhooks](#webhooks)
- [Documentación](#documentación)

---

## Status y Diagnóstico

### `GET /api/Status/health` 🔓

Health check. No requiere auth.

**Respuesta:**
```
200 OK
Healthy
```

---

### `GET /api/Status`

Inicializa el SDK y devuelve el estado de conexión.

**Respuesta:**
```json
{ "status": "Online", "message": "Connected to CONTPAQi SDK" }
```
o
```json
{ "status": "Error", "message": "Failed to initialize SDK", "sdkErrorCode": 3 }
```

---

### `POST /api/Status/connect`

Prueba que el bridge puede abrir una empresa específica.

**Body:**
```json
{ "companyPath": "C:\\Compac\\Empresas\\adMI_EMPRESA" }
```

**Respuesta:**
```json
{ "message": "Successfully connected to company at C:\\Compac\\Empresas\\adMI_EMPRESA" }
```

---

### `GET /api/Status/conceptos?rutaEmpresa=...`

Lista los conceptos de documentos (factura, nota de crédito) de la empresa.

**Respuesta:**
```json
{
  "success": true,
  "conceptos": [
    { "codigo": "4", "nombre": "Factura" },
    { "codigo": "NC", "nombre": "Nota de crédito" }
  ]
}
```

---

### `GET /api/Status/unidades?rutaEmpresa=...`

Devuelve el catálogo de unidades de medida del SDK (texto plano).

---

### `GET /api/Status/documentos?rutaEmpresa=...`

Lista los últimos 10 documentos de la empresa.

---

### `GET /api/Diagnostico/unidades?rutaEmpresa=...`

Alias de `/api/Status/unidades`.

---

## Empresas

### `GET /api/Empresas`

Lista las carpetas dentro de `Contpaqi:EmpresasPath`. Solo lectura del filesystem (no abre CONTPAQi).

**Respuesta:**
```json
{
  "success": true,
  "empresasPath": "C:\\Compac\\Empresas",
  "count": 3,
  "empresas": [
    {
      "nombre": "adMI_EMPRESA",
      "rutaEmpresa": "C:\\Compac\\Empresas\\adMI_EMPRESA",
      "valida": true,
      "tieneCSD": true
    }
  ]
}
```

---

## Clientes

### `POST /api/Clientes`

Crea o actualiza un cliente. Si el código ya existe, actualiza sus datos.

**Body:**
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

**Campos requeridos:** `rutaEmpresa`, `codigo`, `razonSocial`.

**Campos opcionales:** `rfc`, `email`, `calle`, `colonia`, `codigoPostal`, `ciudad`, `estado`, `pais` (default "México"), `regimenFiscal`, `usoCFDI`, `formaPago`.

**Respuesta exitosa:**
```json
{ "success": true, "message": "Cliente CLI001 creado exitosamente", "idCliente": 123 }
```

**Cliente especial "PG" (Público en General):** Si `codigo == "PG"`, el bridge automáticamente completa RFC=`XAXX010101000`, régimen=`616`, usoCFDI=`S01`, formaPago=`01`.

---

### `GET /api/Clientes/{codigo}?rutaEmpresa=...`

Busca un cliente por código. Lee directamente desde SQL.

**Respuesta:**
```json
{
  "success": true,
  "cliente": {
    "contpaqi_id": "123",
    "codigo": "CLI001",
    "razon_social": "Juan Pérez SA de CV",
    "rfc": "JPE950101ABC",
    "email": "...",
    "regimen_fiscal": "601",
    "uso_cfdi": "G03",
    "timestamp": "...",
    "estatus": "1"
  }
}
```
**Si no existe:** `404 Not Found`.

---

## Productos

### `GET /api/Productos?rutaEmpresa=...&limite=20`

Lista los primeros N productos usando el SDK (navegación interna). Más lento pero siempre actualizado.

---

### `GET /api/Productos/{codigo}?rutaEmpresa=...`

Busca un producto por código vía SQL.

---

### `POST /api/Productos`

Crea o actualiza un producto.

**Body:**
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

**Campos requeridos:** `rutaEmpresa`, `codigo`, `nombre`.

**Tipos de producto:** `1` = Producto (inventariable), `2` = Paquete, `3` = Servicio.

**Unidad medida:** Clave SAT (`H87` = pieza, `E48` = servicio, `KGM` = kilogramo, etc.).

---

## Documentos (Facturas)

### `POST /api/Documentos/factura`

Crea una factura (sin timbrar). Devuelve el folio asignado.

**Body:**
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

**Campos requeridos:** `rutaEmpresa`, `codigoConcepto`, `codigoCliente`. `productos` puede ser vacío para facturas de solo cabecera.

**Si el cliente no existe** y se pasan `clienteRazonSocial`, `clienteRFC`, `clienteRegimenFiscal`, se crea automáticamente.

**Si algún producto no existe** y se pasan `nombre`, `unidadMedida`, `claveSAT`, se crea automáticamente.

**UsoCFDI valores comunes:**
- `G01` - Adquisición de mercancías
- `G03` - Gastos en general
- `S01` - Sin efectos fiscales (público en general)
- `CP01` - Pagos

**FormaPago valores:** `01` Efectivo, `03` Transferencia, `99` Por definir.

**MétodoPago:** `PUE` Pago en una sola exhibición, `PPD` Pago en parcialidades o diferido.

**Respuesta exitosa:**
```json
{ "success": true, "message": "Factura creada exitosamente. Serie: A, Folio: 1234", "idDocumento": 5678 }
```

---

### `POST /api/Documentos/timbrar`

Timbra una factura existente ante el SAT/PAC.

**Body:**
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adMI_EMPRESA",
  "codigoConcepto": "4",
  "serie": "A",
  "folio": 1234,
  "passCSD": "contraseña_del_certificado"
}
```

**Importante:** Requiere que la empresa tenga los archivos `.cer` y `.key` configurados en `C:\Compac\Empresas\adMI_EMPRESA\CSD\`.

**Dispara webhook:** `timbrado.exitoso` con `{uuid, serie, folio, codigoConcepto, timestamp}`.

---

### `POST /api/Documentos/validar`

Valida una factura antes de timbrar (sin enviar al SAT). Detecta:
- Falta de CSD o contraseña
- Cliente sin régimen fiscal / código postal / RFC genérico
- Productos sin Clave SAT
- Documento ya cancelado
- Campos CFDI 4.0 faltantes

**Body:** igual a `/api/Documentos/timbrar`.

**Respuesta:**
```json
{
  "success": true,
  "listoParaTimbrar": false,
  "errores": 2,
  "warnings": 1,
  "issues": [
    { "severidad": "Error", "categoria": "Producto", "mensaje": "Producto 'PROD001' no tiene Clave SAT..." }
  ]
}
```

---

### `GET /api/Documentos/xml?rutaEmpresa=...&codigoConcepto=4&serie=A&folio=1234&formato=0`

Devuelve el contenido del XML CFDI timbrado. `formato=1` para PDF.

---

### `GET /api/Documentos/uuid?rutaEmpresa=...&codigoConcepto=4&serie=A&folio=1234`

Devuelve el UUID (folio fiscal) del CFDI timbrado.

**Respuesta:**
```json
{ "success": true, "uuid": "5EB7E3A5-4B0E-4F8E-9F1A-3B5F6E7D8C9A" }
```

---

### `POST /api/Documentos/datos-cfdi`

Lee un dato específico del CFDI timbrado.

**Body:**
```json
{
  "rutaEmpresa": "...",
  "codigoConcepto": "4",
  "serie": "A",
  "folio": 1234,
  "password": "...",
  "dato": 2
}
```

**Valores de `dato`:**
| Valor | Significado |
|---|---|
| 1 | SerieCertificadoEmisor |
| 2 | UUID |
| 3 | SerieCertificadoSAT |
| 4 | FechaHoraCertificación |
| 5 | SelloDigitalCFDI |
| 6 | SelloSAT |
| 7 | CadenaOriginalSAT |
| 8 | MétodoPago |
| 9 | LugarExpedición |
| 10 | RégimenFiscal |

---

### `POST /api/Documentos/cancelar`

Cancela un CFDI 4.0 ante el SAT.

**Body:**
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

**Motivos de cancelación (SAT):**
| Código | Descripción | Requiere UUID |
|---|---|---|
| `01` | Comprobante emitido con errores con relación | Sí |
| `02` | Comprobante emitido con errores sin relación | No |
| `03` | No se llevó a cabo la operación | No |
| `04` | Operación nominativa en factura global | No |

**Respuesta:**
```json
{ "success": true, "message": "Documento cancelado exitosamente", "acuse": "<xml>...</xml>" }
```

**Dispara webhook:** `cancelacion.exitosa`.

---

### `POST /api/Documentos/cancelar-admin`

Cancela un documento solo en CONTPAQi (NO afecta al SAT). Útil para errores administrativos.

---

### `POST /api/Documentos/saldar`

Asocia un pago (CxC) con un documento a pagar. Para complementos de pago PPD.

**Body:**
```json
{
  "rutaEmpresa": "...",
  "codConceptoPagar": "4",
  "seriePagar": "A",
  "folioPagar": 100,
  "codConceptoPago": "PAGO",
  "seriePago": "P",
  "folioPago": 1,
  "importe": 1500.00,
  "idMoneda": 1,
  "fecha": "01/15/2024"
}
```

---

### `GET /api/Documentos/ultimos?rutaEmpresa=...&cantidad=10`

Lista los últimos N documentos vía navegación SDK.

---

## Integración (flujo completo)

### `POST /api/Integracion/flujo-completo` ⭐

**El endpoint estrella.** Crea cliente + producto + factura + timbra en **una sola llamada**.

**Body:**
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
    "nombre": "Servicio de consultoría",
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

**Comportamiento:**
1. Crea/actualiza el cliente (si existe, lo actualiza).
2. Crea/actualiza el producto.
3. Crea la factura con los productos.
4. Si `passCSD` viene, la timbra.
5. Dispara webhook `timbrado.exitoso` si timbró correctamente.

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

## Sincronización bidireccional

Estos endpoints son para sincronizar Laravel (u otro sistema) con CONTPAQi.

### `GET /api/Sync/clientes?rutaEmpresa=...&limite=500`

Snapshot completo de clientes vía SQL.

### `GET /api/Sync/clientes/modificados?rutaEmpresa=...&desde=2024-01-01T00:00:00&limite=500`

Solo clientes cambiados desde una fecha. Usa el campo `CTIMESTAMP` de la tabla `admClientes`.

### `GET /api/Sync/productos?rutaEmpresa=...&limite=500`

Snapshot completo de productos.

### `GET /api/Sync/productos/modificados?rutaEmpresa=...&desde=...`

Productos modificados desde fecha.

### `GET /api/Sync/documentos/modificados?rutaEmpresa=...&desde=...` ⭐

**Documentos modificados** (facturas, notas, etc.). El endpoint clave para sincronizar ventas desde CONTPAQi hacia Laravel.

Cada documento trae: `contpaqi_id`, `serie`, `folio`, `uuid`, `fecha`, `cliente_codigo`, `importe`, `iva`, `total`, `metodo_pago`, `forma_pago`, `uso_cfdi`, `cancelado`, `folio_sat`, `timestamp`.

### `POST /api/Sync/clientes/batch`

Push masivo Laravel → CONTPAQi.

**Body:**
```json
{
  "rutaEmpresa": "...",
  "items": [
    { "codigo": "CLI001", "razonSocial": "...", "rfc": "...", "regimenFiscal": "601", ... },
    { "codigo": "CLI002", "razonSocial": "...", "rfc": "...", ... }
  ]
}
```

**Respuesta:**
```json
{
  "success": true,
  "total": 2,
  "ok": 2,
  "error": 0,
  "resultados": [
    { "codigo": "CLI001", "exito": true, "mensaje": "...", "idCliente": 123 },
    { "codigo": "CLI002", "exito": false, "mensaje": "..." }
  ]
}
```

### `POST /api/Sync/productos/batch`

Igual que clientes pero para productos.

### `GET /api/Sync/status?rutaEmpresa=...`

Diagnóstico: valida que el SDK se puede inicializar, la empresa se abre, y devuelve conteos.

---

## Reportes

### `GET /api/Reportes/ventas?rutaEmpresa=...&desde=2024-01-01&hasta=2024-01-31`

Ventas agrupadas por día.

**Respuesta:**
```json
{
  "success": true,
  "desde": "2024-01-01T00:00:00",
  "hasta": "2024-01-31T00:00:00",
  "totalGeneral": 125430.50,
  "count": 31,
  "ventas": [
    { "fecha": "2024-01-31", "documentos": 12, "importe": 10000, "iva": 1600, "total": 11600 }
  ]
}
```

### `GET /api/Reportes/top-clientes?rutaEmpresa=...&desde=...&hasta=...&top=10`

Top N clientes por ventas totales en un periodo.

### `GET /api/Reportes/top-productos?rutaEmpresa=...&desde=...&hasta=...&top=10`

Top N productos más vendidos.

---

## Webhooks

### `POST /api/Webhooks`

Registra un webhook.

**Body:**
```json
{
  "evento": "timbrado.exitoso",
  "url": "https://tu-laravel.com/webhooks/contpaqi"
}
```

**Eventos disponibles:**
| Evento | Cuándo se dispara |
|---|---|
| `timbrado.exitoso` | Factura timbrada OK ante SAT |
| `timbrado.fallido` | Error al timbrar |
| `cancelacion.exitosa` | Factura cancelada ante SAT |
| `cancelacion.fallida` | Error al cancelar |
| `documento.creado` | Nuevo documento creado |
| `*` | Todos los eventos (catch-all) |

**Payload típico de `timbrado.exitoso`:**
```json
{
  "evento": "timbrado.exitoso",
  "timestamp": "2024-01-15T18:30:00Z",
  "payload": {
    "rutaEmpresa": "...",
    "codigoConcepto": "4",
    "serie": "A",
    "folio": 1234,
    "uuid": "5EB7E3A5-...",
    "timestamp": "2024-01-15T18:30:00Z"
  }
}
```

### `GET /api/Webhooks`

Lista todos los webhooks registrados (en memoria; se pierden al reiniciar el bridge).

### `POST /api/Webhooks/emit`

Emite un webhook manualmente (testing).

---

## Documentación

### `GET /api/Docs` 🔓

Página HTML con la lista completa de endpoints, descripción y ejemplos.

### `GET /api/Docs/openapi.json` 🔓

Manifiesto OpenAPI 3.0 (simplificado). Importable a Postman, Insomnia, etc.