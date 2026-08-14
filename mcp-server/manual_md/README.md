# Manual del SDK de CONTPAQi® — Recursos MCP

Este directorio contiene el contenido del **Manual de Referencia del SDK de CONTPAQi®**
[publicado por CONTPAQi](https://conocimiento.blob.core.windows.net/conocimiento/Manuales/MR_SDK/),
convertido a Markdown y listo para servirse como **resources MCP** desde `server.js`.

## Contenido

| Archivo | Descripción |
|---|---|
| `*.md` | Un archivo Markdown por capítulo del manual (~70 capítulos, ~280 KB total) |
| `index.json` | Metadatos de todos los capítulos (uri, nombre, tamaño, URL fuente) |

## Origen

- URL base: `https://conocimiento.blob.core.windows.net/conocimiento/Manuales/MR_SDK/`
- Última modificación según CONTPAQi: 30 de junio de 2026
- Tecnología original: generado por Dr.Explain

## Cómo se sirve al LLM

El servidor MCP (`../server.js`) expone este contenido de tres formas:

### 1. Como resources MCP (recomendado)

| Método MCP | Descripción |
|---|---|
| `resources/list` | Devuelve los 69 capítulos con su uri, nombre y mimeType |
| `resources/read` (uri `manual://<slug>`) | Devuelve el Markdown de un capítulo |
| `resources/templates/list` | Plantilla `manual://{slug}` |

### 2. Como tools (para clientes sin soporte de resources)

| Tool | Uso |
|---|---|
| `contpaqi_sdk_manual_list` | Lista capítulos (con filtro opcional) |
| `contpaqi_sdk_manual_get` | Obtiene Markdown por uri o slug |
| `contpaqi_sdk_manual_search` | Búsqueda full-text en todo el manual |
| `contpaqi_sdk_manual_overview` | Índice compacto para inyectar en contexto |

## Capítulos principales (slug → uri)

| Slug | Contenido |
|---|---|
| `introduccion` | Qué es el SDK y cómo funciona |
| `requerimientos_para_trabajar_con_el_sdk` | Prerrequisitos |
| `recomendaciones_y_consideraciones_importantes` | Buenas prácticas |
| `funciones_obligatorias` | Flujo mínimo: Init → AbreEmpresa → … → Termina SDK |
| `inicializacion___terminacion` | `fInicializaSDK`, `fTerminaSDK` |
| `apertura___cierre` | `fAbreEmpresa`, `fCierraEmpresa` |
| `manejo_de_errores` | `fError`, `rError`, `fErrorDescripcion` |
| `tipos_de_datos_abstractos_del_sdk` | Tipos SDK (tDocumento, tMovimiento, etc.) |
| `equivalencias_de_tipos_de_datos` | Mapa entre tipos SDK ↔ lenguajes |
| `constantes_del_sdk` | `kSIN_ERRORES`, `kVERDADERO`, etc. |
| `manejo_de_errores` | Cómo atrapar y reportar errores |
| `navegacion` | `fPosAnterior`, `fPosPrimero`, etc. |
| `trabajando_con_documentos` | Conceptos sobre documentos CFDI |
| `trabajando_con_productos_o_clientes` | Conceptos sobre productos/clientes |
| `timbrar_documentos` | Proceso de timbrado |
| `casos_practicos` | Ejemplos end-to-end (factura gasolina, REP, carta porte, etc.) |

Las funciones individuales están en páginas como:
- `bajo_nivel___lectura_escritura__1..12` (CRUD bajo nivel: `fInsertarDocumento`, `fEditarDocumento`, etc.)
- `alto_nivel___lectura_escritura__1..6` (CRUD alto nivel: `fAltaDocumento`, `fAfectaDocto`, etc.)
- `bajo_nivel___busqueda_navegacion_1..8` (Búsqueda/navegación bajo nivel)
- `alto_nivel___busqueda_navegacion` (Búsqueda alto nivel)

## Re-generar el manual

Para actualizar cuando CONTPAQi publique una nueva versión:

```bash
# Desde mcp-server/
node convert_manual.js --download
```

Esto descarga los HTML y regenera los `.md` + `index.json` automáticamente.
