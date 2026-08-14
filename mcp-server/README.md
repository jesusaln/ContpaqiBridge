# ContpaqiBridge MCP Server

Servidor MCP (Model Context Protocol) que envuelve el [ContpaqiBridge](../README.md) y expone sus funcionalidades como **tools** + **resources** para asistentes IA como Claude, opencode, etc.

## ¿Qué es MCP?

[Model Context Protocol](https://modelcontextprotocol.io/) es un protocolo estándar abierto que permite a los asistentes IA invocar herramientas externas. El servidor MCP se comunica vía **JSON-RPC 2.0** sobre stdio (entrada/salida estándar).

```
┌────────────────┐       JSON-RPC 2.0 (stdio)       ┌──────────────────┐
│  Claude /      │◄────────────────────────────────►│  MCP Server       │
│  opencode /    │   {"method":"tools/call",        │  (server.js)      │
│  cualquier LLM │    "params":{"name":"..."}}      │  Node.js puro     │
└────────────────┘                                  └────────┬──────────┘
                                                              │ HTTP + X-Api-Key
                                                              │
                                                     ┌────────▼─────────┐
                                                     │ ContpaqiBridge   │
                                                     │ (.NET, puerto    │
                                                     │  5000)           │
                                                     └──────────────────┘
```

## ✨ Características

- **Sin dependencias externas**: solo Node.js (>= 18).
- **30 tools** que cubren todo el bridge: clientes, productos, facturas, sync, reportes, webhooks y **manual del SDK**.
- **69 resources** con el contenido completo del Manual de Referencia del SDK de CONTPAQi (vía `resources/list` + `resources/read`).
- **Compatible** con cualquier cliente MCP: opencode, Claude Desktop, Cursor, Zed, etc.
- **Type-safe**: cada tool tiene un `inputSchema` JSON-Schema estricto.

## 📚 Manual del SDK embebido

El servidor incluye el [Manual de Referencia del SDK de CONTPAQi®](https://conocimiento.blob.core.windows.net/conocimiento/Manuales/MR_SDK/) como **resources MCP** (scheme `manual://`). El LLM puede consultarlo bajo demanda sin tener que pedirlo de nuevo.

### Como resources (recomendado)

| Método MCP | Uso |
|---|---|
| `resources/list` | Lista los 69 capítulos del manual |
| `resources/read` (uri `manual://<slug>`) | Obtiene el Markdown de un capítulo |
| `resources/templates/list` | Plantilla `manual://{slug}` |

### Como tools (para clientes sin soporte de resources)

| Tool | Descripción |
|---|---|
| `contpaqi_sdk_manual_list` | Lista capítulos, con filtro opcional |
| `contpaqi_sdk_manual_get` | Obtiene el Markdown por uri o slug |
| `contpaqi_sdk_manual_search` | Búsqueda full-text en todo el manual |
| `contpaqi_sdk_manual_overview` | Índice compacto tipo TOC |

### Capítulos destacados

- `manual://introduccion` - Qué es el SDK
- `manual://requerimientos_para_trabajar_con_el_sdk`
- `manual://funciones_obligatorias` - Flujo mínimo (Init/Abre/Cierra)
- `manual://tipos_de_datos_abstractos_del_sdk` - 17 KB de tipos
- `manual://constantes_del_sdk` - Códigos de error y constantes
- `manual://manejo_de_errores` - Patrón try/catch con `fError` / `rError`
- `manual://bajo_nivel___lectura_escritura__1..12` - CRUD de documentos
- `manual://alto_nivel___lectura_escritura__1..6` - Funciones de alto nivel
- `manual://casos_practicos` - Ejemplos end-to-end (factura gasolina, REP, carta porte, etc.)

Usa `contpaqi_sdk_manual_list` para descubrir todos.

## 🚀 Instalación

### 1. Asegúrate de que el bridge está corriendo

```bash
cd ContpaqiBridge
./start_bridge.ps1
# Verifica: curl http://localhost:5000/api/Status/health → "Healthy"
```

### 2. Configurar variables de entorno

Edita `opencode.json` o exporta:

```bash
export CONTPAQI_BRIDGE_URL=http://localhost:5000
export CONTPAQI_API_KEY=tu_clave_de_appsettings_json
```

> 💡 La API Key debe coincidir con `Bridge:ApiKey` en el `appsettings.json` del bridge.

### 3. Registrar el MCP server

#### En opencode (`opencode.json`)

```json
{
  "mcp": {
    "contpaqi-bridge": {
      "type": "local",
      "command": ["node", "mcp-server/server.js"],
      "enabled": true,
      "environment": {
        "CONTPAQI_BRIDGE_URL": "http://localhost:5000",
        "CONTPAQI_API_KEY": "TU_API_KEY_DE_APPSETTINGS_JSON"
      }
    }
  }
}
```

#### En Claude Desktop (`claude_desktop_config.json`)

```json
{
  "mcpServers": {
    "contpaqi-bridge": {
      "command": "node",
      "args": ["C:\\Users\\JESUS\\Desktop\\ContpaqiBridge-master\\ContpaqiBridge-master\\mcp-server\\server.js"],
      "env": {
        "CONTPAQI_BRIDGE_URL": "http://localhost:5000",
        "CONTPAQI_API_KEY": "tu_clave"
      }
    }
  }
}
```

#### En Cursor / Zed / otros

Similar: command = `node`, args = ruta absoluta al `server.js`.

## 📚 Tools disponibles

| Tool | Descripción |
|---|---|
| `contpaqi_health_check` | Health check del bridge |
| `contpaqi_status` | Inicializar SDK y devolver estado |
| `contpaqi_list_empresas` | Listar empresas disponibles |
| `contpaqi_buscar_cliente` | Buscar cliente por código |
| `contpaqi_crear_cliente` | Crear/actualizar cliente |
| `contpaqi_buscar_producto` | Buscar producto por código |
| `contpaqi_crear_producto` | Crear/actualizar producto |
| `contpaqi_crear_factura` | Crear factura (sin timbrar) |
| `contpaqi_timbrar_factura` | Timbrar ante SAT |
| `contpaqi_validar_factura` | Validar antes de timbrar |
| `contpaqi_obtener_xml` | Descargar XML/PDF |
| `contpaqi_obtener_uuid` | Obtener UUID (folio fiscal) |
| `contpaqi_cancelar_factura` | Cancelar CFDI 4.0 |
| `contpaqi_flujo_completo` | ⭐ Cliente + producto + factura + timbrado |
| `contpaqi_pull_clientes` | Snapshot completo de clientes |
| `contpaqi_pull_clientes_modificados` | Sync incremental clientes |
| `contpaqi_pull_productos` | Snapshot de productos |
| `contpaqi_pull_productos_modificados` | Sync incremental productos |
| `contpaqi_pull_facturas` | Traer facturas nuevas desde CONTPAQi |
| `contpaqi_push_clientes_batch` | Push masivo Laravel → CONTPAQi |
| `contpaqi_push_productos_batch` | Push masivo productos |
| `contpaqi_reporte_ventas` | Ventas por día en periodo |
| `contpaqi_reporte_top_clientes` | Top N clientes por ventas |
| `contpaqi_reporte_top_productos` | Top N productos vendidos |
| `contpaqi_registrar_webhook` | Registrar webhook |
| `contpaqi_listar_webhooks` | Listar webhooks |
| `contpaqi_sdk_manual_list` | Lista capítulos del Manual SDK (con filtro) |
| `contpaqi_sdk_manual_get` | Obtiene Markdown de un capítulo |
| `contpaqi_sdk_manual_search` | Búsqueda full-text en el manual |
| `contpaqi_sdk_manual_overview` | Índice compacto tipo TOC |

## 🧪 Probar manualmente

```bash
# Inicializar
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{}}}' \
  | node mcp-server/server.js

# Listar tools
echo '{"jsonrpc":"2.0","id":2,"method":"tools/list"}' \
  | node mcp-server/server.js

# Llamar a un tool
echo '{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"contpaqi_health_check","arguments":{}}}' \
  | node mcp-server/server.js
```

## 💡 Ejemplos de uso desde Claude / opencode

Una vez configurado, puedes pedirle a Claude cosas como:

- **"Crea el cliente CLI-001 con RFC XAXX010101000, régimen 616, en la empresa C:\Compac\Empresas\adMI_EMPRESA"**
  → Claude usará `contpaqi_crear_cliente` con los datos que proporcionaste.

- **"¿Cuántas facturas se timbraron en enero 2024 y cuál fue el total?"**
  → Claude llamará a `contpaqi_reporte_ventas`.

- **"Cancela la factura A-1234 con motivo 02"**
  → Claude llamará a `contpaqi_cancelar_factura`.

- **"Sincroniza los clientes nuevos desde CONTPAQi hacia mi sistema"**
  → Claude usará `contpaqi_pull_clientes_modificados`.

- **"Haz el flujo completo: vende 1 licencia mensual a Juan Pérez por $1500, timbra y devuélveme el folio y UUID"**
  → Claude usará `contpaqi_flujo_completo` en una sola llamada.

- **"¿Cómo registro un CFDI de relación desde el SDK?"**
  → Claude consultará el resource `manual://registrar_una_relacion_cfdi_mediante_uuid`.

- **"¿Cuáles son las funciones obligatorias para inicializar el SDK?"**
  → Claude usará `contpaqi_sdk_manual_get` con slug `funciones_obligatorias`.

- **"Busca en el manual cómo se maneja el error 42"**
  → Claude llamará `contpaqi_sdk_manual_search` con query `42`.

## 🔧 Personalizar

Edita `server.js` y agrega más tools siguiendo el patrón:

```javascript
{
    name: 'contpaqi_mi_tool',
    description: 'Qué hace',
    inputSchema: {
        type: 'object',
        properties: {
            // parámetros con sus tipos
        },
        required: ['param1']
    },
    handler: async (params) => callBridge('POST', '/api/Mi/Endpoint', params)
}
```

## 🔄 Actualizar el manual embebido

El manual se descarga y convierte desde
`https://conocimiento.blob.core.windows.net/conocimiento/Manuales/MR_SDK/`.

Para re-importarlo (por ejemplo cuando salga una nueva versión):

```bash
# 1. Descargar las páginas HTML al directorio sdk_pages/
node convert_manual.js --download

# 2. Convertir HTML → Markdown y regenerar el índice
node convert_manual.js

# 3. Reiniciar el servidor MCP para que recargue el índice
```

Los archivos `.md` resultantes quedan en `manual_md/` y se sirven como resources.

## ⚙️ Troubleshooting

### El server no responde

Verifica:
1. El bridge está corriendo (`curl http://localhost:5000/api/Status/health`).
2. `CONTPAQI_API_KEY` coincide con `Bridge:ApiKey` en `appsettings.json`.
3. Node.js >= 18 instalado (`node --version`).
4. Si los resources del manual no aparecen, ejecuta `node convert_manual.js` para regenerar `manual_md/index.json`.

### "Tool no encontrada"

El tool no existe o tiene un typo. Usa `tools/list` para ver los nombres exactos.

### "Error ejecutando X: HTTP 403"

API Key inválida. Verifica que `CONTPAQI_API_KEY` es exactamente igual a la del bridge.

### "Error de red: connect ECONNREFUSED"

El bridge no está corriendo o la URL es incorrecta. Verifica `CONTPAQI_BRIDGE_URL`.