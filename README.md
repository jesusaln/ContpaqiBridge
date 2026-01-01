# ContpaqiBridge

Este proyecto es un puente (API REST) que conecta sistemas externos (como Laravel, VPS, WebApps) con el SDK de CONTPAQi Comercial, permitiendo facturar, crear clientes y productos de manera remota.

## 🚀 Instalación y Puesta en Marcha

### 1. Requisitos Previos
- Servidor Windows con CONTPAQi Comercial instalado y funcioando.
- Licencia de CONTPAQi activa.
- .NET 6.0 SDK instalado.
- ZeroTier instalado (para acceso remoto).

### 2. Configuración de Red (ZeroTier)

Para que tu servidor remoto (Laravel/VPS) vea esta máquina, usamos ZeroTier.

1. **Obtener Node ID**: Ejecuta `zerotier-cli info` en PowerShell como Admin.
2. **Unirse a la Red**: `zerotier-cli join <TU_ID_DE_RED>`.
3. **Autorizar**: Ve a [ZeroTier Central](https://my.zerotier.com/), busca la red y autoriza a ambos dispositivos (Local y Remoto).
4. **Abrir Firewall**:
   Abre PowerShell como Administrador y ejecuta:
   ```powershell
   New-NetFirewallRule -DisplayName "Permitir ContpaqiBridge 5000" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
   ```

**Tu IP de ZeroTier detectada es**: `192.168.191.226`

### 3. Ejecución
Para iniciar el bridge escuchando en todas las interfaces:

```powershell
./start_bridge.ps1
```
o manualmente:
```bash
dotnet run
```

La URL base será: `http://192.168.191.226:5000`

---

## 📚 Documentación de API

### Endpoint Principal (Flujo Completo)
Este es el endpoint recomendado para integraciones. Crea el cliente y producto si no existen, y luego genera y timbra la factura en un solo paso.

**POST** `/api/Integracion/flujo-completo`

**Ejemplo JSON:**
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adTU_EMPRESA",
  "cliente": {
    "codigo": "CTE001",
    "razonSocial": "Empresa Cliente S.A. de C.V.",
    "rfc": "XAXX010101000",
    "email": "cliente@email.com",
    "calle": "Av. Principal 123",
    "colonia": "Centro",
    "codigoPostal": "44100",
    "regimenFiscal": "601",
    "usoCFDI": "G03",
    "formaPago": "99"
  },
  "producto": {
    "codigo": "SERV01",
    "nombre": "Servicio de Mantenimiento",
    "precio": 1500.00,
    "claveSAT": "81101500",
    "unidadMedida": "E48"
  },
  "factura": {
    "codigoConcepto": "4CLIMAS",
    "cantidad": 1,
    "passCSD": "tu_contraseña_csd",
    "usoCFDI": "G03",
    "formaPago": "99",
    "metodoPago": "PUE"
  }
}
```

---

### Endpoints Individuales

Si prefieres realizar las operaciones paso a paso:

#### 1. Crear Cliente
**POST** `/api/Clientes`
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adTU_EMPRESA",
  "codigo": "CTE001",
  "razonSocial": "Cliente Prueba",
  "rfc": "XAXX010101000",
  "codigoPostal": "44100",
  "regimenFiscal": "616",
  "usoCFDI": "S01",
  "formaPago": "01"
}
```

#### 2. Crear Producto
**POST** `/api/Productos`
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adTU_EMPRESA",
  "codigo": "PROD01",
  "nombre": "Producto 1",
  "precio": 100.00,
  "claveSAT": "01010101",
  "unidadMedida": "H87"
}
```

#### 3. Crear Factura (Sin timbrar)
**POST** `/api/Documentos/factura`
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adTU_EMPRESA",
  "codigoConcepto": "4CLIMAS",
  "codigoCliente": "CTE001",
  "productos": [
    { "codigo": "PROD01", "cantidad": 1, "precio": 100.00 }
  ]
}
```

#### 4. Timbrar Factura
**POST** `/api/Documentos/timbrar`
```json
{
  "rutaEmpresa": "C:\\Compac\\Empresas\\adTU_EMPRESA",
  "codigoConcepto": "4CLIMAS",
  "folio": 1234,
  "passCSD": "tu_contraseña"
}
```

#### 5. Obtener XML de Factura
Recupera el contenido XML de una factura ya timbrada.

**GET** `/api/Documentos/xml`

**Query Params:**
- `rutaEmpresa`: Ruta de la empresa.
- `codigoConcepto`: Código del concepto.
- `serie`: Serie de la factura (opcional).
- `folio`: Folio de la factura.

Respuesta: `{"success": true, "xml": "<xml>...</xml>"}`

#### 6. Verificar Estado
**GET** `/api/Status`
Respuesta: `{"status": "Online", "message": "Connected to CONTPAQi SDK"}`
