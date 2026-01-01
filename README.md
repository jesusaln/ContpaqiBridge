# ContpaqiBridge 🚀

Este proyecto es un puente (API REST) que conecta sistemas externos (como Laravel en VPS, Hostinger o WebApps) con el SDK de CONTPAQi Comercial LOCAL. Permite facturar, crear clientes, productos y extraer XMLs de manera remota a través de internet.

## 🛠️ Instalación y Puesta en Marcha

### 1. Requisitos Previos
- Servidor Windows con CONTPAQi Comercial instalado y funcionando.
- .NET 6.0 SDK (Runtime x86 importante).
- ZeroTier instalado para el tunel VPN.

### 2. Configuración de Red (Acceso Remoto)

Para que tu sistema de ventas (ej. Laravel en VPS) se comunique con esta máquina local:

1. **ZeroTier**:
   - Instala ZeroTier en esta máquina y en tu VPS.
   - Únete a la misma red en ambos lados.
   - Autoriza los nodos en [my.zerotier.com](https://my.zerotier.com/).
2. **Abrir Puerto 5000**:
   Ejecuta esto en PowerShell como Administrador:
   ```powershell
   New-NetFirewallRule -DisplayName "ContpaqiBridge" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
   ```
3. **Tu IP Remota**: `192.168.191.226`

### 3. Ejecución
Usa el script automatizado que limpia procesos previos e inicia el bridge:
```powershell
./start_bridge.ps1
```

---

## 📚 Documentación de API

### 📄 Obtener XML de Factura
Recupera el contenido XML de una factura ya timbrada. El bridge utiliza funciones de alto nivel del SDK para extraer el archivo directamente desde la carpeta interna `XML_SDK` de la empresa.

**Endpoint:** `GET /api/Documentos/xml`

| Parámetro | Tipo | Descripción |
| :--- | :--- | :--- |
| `rutaEmpresa` | string | Ruta completa (C:\Compac\Empresas\...) |
| `codigoConcepto`| string | Código del concepto (ej: "4") |
| `serie` | string | (Opcional) Serie de la factura |
| `folio` | double | Folio de la factura |

**Ejemplo:**
`http://192.168.191.226:5000/api/Documentos/xml?rutaEmpresa=C:\Compac\Empresas\adEmpresa&codigoConcepto=4&serie=AV&folio=1401`

**Respuesta:**
```json
{
  "success": true,
  "mensaje": "XML obtenido correctamente",
  "xml": "<?xml version=\"1.0\" ... </cfdi:Comprobante>"
}
```

---

### 📝 Creación y Timbrado (Flujo Completo)
Envía un solo JSON y el bridge se encarga de:
1. Crear el cliente (si no existe).
2. Crear el producto (si no existe).
3. Generar la factura.
4. Timbrar el CFDI.

**Endpoint:** `POST /api/Integracion/flujo-completo`

```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adTU_EMPRESA",
  "cliente": { "codigo": "CTE01", "razonSocial": "Juan Perez", "rfc": "XAXX010101000", "regimenFiscal": "616", "usoCFDI": "S01" },
  "producto": { "codigo": "001", "nombre": "Suscripción", "precio": 100.00, "claveSAT": "01010101" },
  "factura": { "codigoConcepto": "4", "passCSD": "tu_password", "metodoPago": "PUE", "formaPago": "99" }
}
```
---

### ❌ Cancelar Factura ante el SAT
Cancela un documento CFDI 4.0 con motivo oficial del SAT.

**Endpoint:** `POST /api/Documentos/cancelar`

| Motivo | Descripción |
| :--- | :--- |
| `01` | Con errores CON relación (requiere `uuidSustitucion`) |
| `02` | Con errores SIN relación |
| `03` | No se llevó a cabo la operación |
| `04` | Operación en factura global |

```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adTU_EMPRESA",
  "codigoConcepto": "4",
  "serie": "AV",
  "folio": 1401,
  "motivoCancelacion": "02",
  "passCSD": "tu_password",
  "uuidSustitucion": ""
}
```

---

### 🗑️ Cancelar Solo en CONTPAQi (Administrativa)
Cancela el documento localmente sin afectar al SAT. Útil para errores internos.

**Endpoint:** `POST /api/Documentos/cancelar-admin`

```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adTU_EMPRESA",
  "codigoConcepto": "4",
  "serie": "AV",
  "folio": 1401
}
```

---

## 🔧 Solución de Problemas (Troubleshooting)

- **Error 3 (CACSql.dll)**: El SDK requiere que el Bridge corra en modo `x86`. Si ves este error, asegúrate de que el PATH incluya la carpeta de Comercial y de inicializar el Bridge con `./start_bridge.ps1`.
- **Fatal Error 0xC0000005**: Suele suceder al pasar estructuras mal alineadas. El sistema de XML ahora usa funciones de "Alto Nivel" para evitar esto.
- **Archivo XML no encontrado**: Algunos SDKs guardan el XML en la subcarpeta `XML_SDK` dentro de la empresa. El bridge ya escanea esa carpeta automáticamente.

---
*Desarrollado para la integración Laravel-Contpaqi por Antigravity AI.*
