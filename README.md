# Contpaqi Bridge API Documentation

Esta API permite la creación automática de clientes, productos y facturas en CONTPAQi Comercial, incluyendo el timbrado con CFDI 4.0.

## Endpoint Principal

`POST /api/integracion/flujo-completo`

## Estructura del JSON

El cuerpo de la solicitud debe contener los siguientes objetos principales:

### 1. Configuración General (`rutaEmpresa`)
Ruta completa a la carpeta de la empresa en el servidor.

```json
"rutaEmpresa": "C:\\Compac\\Empresas\\adEJEMPLO"
```

### 2. Cliente (`cliente`)
Datos del cliente. Si el cliente ya existe, solo se verifica su existencia (no se actualiza, excepto para PG).

| Campo | Tipo | Obligatorio | Descripción |
| :--- | :--- | :--- | :--- |
| `codigo` | String | Sí | Código único del cliente. |
| `razonSocial` | String | Sí | Nombre o Razón Social (sin Régimen Capital para CFDI 4.0). |
| `rfc` | String | Sí | RFC válido. |
| `codigoPostal` | String | Sí | CP Domicilio Fiscal. |
| `regimenFiscal` | String | No* | Clave del Régimen Fiscal (ej. "601"). *Obligatorio si es nuevo.* |
| `usoCFDI` | String | No* | Clave Uso CFDI por defecto (ej. "G03"). *Obligatorio si es nuevo.* |
| `formaPago` | String | No | Clave Forma de Pago por defecto (ej. "99"). |

**Caso Especial: Público en General**
Si envías `"codigo": "PG"`, el sistema forzará automáticamente:
- `razonSocial`: "PUBLICO EN GENERAL"
- `rfc`: "XAXX010101000"
- `regimenFiscal`: "616"
- `usoCFDI`: "S01"
- `formaPago`: "01" (si no envías otra)

### 3. Producto (`producto`)
Datos del producto/servicio.

| Campo | Tipo | Obligatorio | Descripción |
| :--- | :--- | :--- | :--- |
| `codigo` | String | Sí | Código único del producto. |
| `nombre` | String | Sí | Descripción del producto. |
| `precio` | Decimal | Sí | Precio unitario antes de impuestos. **Si el producto ya existe con precio 0, se actualizará a este precio.** |
| `claveSAT` | String | No | Clave de Producto/Servicio SAT (ej. "84111506"). |
| `unidadMedida` | String | No | Nombre de la unidad (ej. "Pieza", "Servicio", "H87"). Defecto: "H87". |

### 4. Factura (`factura`)
Datos para la generación del documento.

| Campo | Tipo | Obligatorio | Descripción |
| :--- | :--- | :--- | :--- |
| `codigoConcepto` | String | Sí | Código del concepto de factura en CONTPAQi (ej. "4"). |
| `passCSD` | String | Sí* | Contraseña del certificado digital. *Si se omite, crea la factura pero NO timbra.* |
| `cantidad` | Decimal | Sí | Cantidad de productos a facturar. |
| `usoCFDI` | String | Sí | Clave Uso CFDI para esta factura (ej. "G03", "S01"). |
| `metodoPago` | String | Sí | "PUE" (Pago en una sola exhibición) o "PPD" (Pago en parcialidades). |
| `formaPago` | String | Sí | Clave Forma de Pago (ej. "01", "03", "99"). |
| `observaciones` | String | No | Comentarios adicionales en la factura. |

## Ejemplo Completo (JSON)

```json
{
    "rutaEmpresa": "C:\\Compac\\Empresas\\adJESUS_LOPEZ_NORIEGA",
    "cliente": {
        "codigo": "CTE001",
        "razonSocial": "EMPRESA EJEMPLO",
        "rfc": "XAXX010101000",
        "codigoPostal": "83000",
        "regimenFiscal": "616",
        "usoCFDI": "S01",
        "formaPago": "01"
    },
    "producto": {
        "codigo": "FEE01",
        "nombre": "HONORARIOS",
        "precio": 500.00,
        "claveSAT": "84111506",
        "unidadMedida": "H87"
    },
    "factura": {
        "codigoConcepto": "4",
        "passCSD": "tu_contraseña_aqui",
        "cantidad": 1,
        "usoCFDI": "S01",
        "metodoPago": "PUE",
        "formaPago": "01",
        "observaciones": "Factura generada desde API"
    }
}
```
