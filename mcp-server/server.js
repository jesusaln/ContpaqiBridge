#!/usr/bin/env node
/**
 * ContpaqiBridge MCP Server
 *
 * Servidor MCP (Model Context Protocol) que envuelve la API HTTP del
 * ContpaqiBridge y expone sus funcionalidades como tools para asistentes IA.
 *
 * Comunicación: JSON-RPC 2.0 sobre stdio (entrada/salida estándar).
 *
 * Sin dependencias externas: solo usa Node.js stdlib.
 *
 * Variables de entorno:
 *   CONTPAQI_BRIDGE_URL  URL del bridge (default: http://localhost:5000)
 *   CONTPAQI_API_KEY     API Key del bridge (default: vacío)
 */

'use strict';

const http = require('http');
const https = require('https');
const fs = require('fs');
const path = require('path');
const { URL } = require('url');

// =====================================================================
// CONFIGURACIÓN
// =====================================================================

const BRIDGE_URL = process.env.CONTPAQI_BRIDGE_URL || 'http://localhost:5000';
const API_KEY = process.env.CONTPAQI_API_KEY || '';
const SERVER_NAME = 'contpaqi-bridge';
const SERVER_VERSION = '1.1.0';

const bridgeUrl = new URL(BRIDGE_URL);

// =====================================================================
// CLIENTE HTTP HACIA EL BRIDGE
// =====================================================================

function callBridge(method, path, body = null, query = null) {
    return new Promise((resolve, reject) => {
        const url = new URL(path, bridgeUrl);
        if (query) {
            for (const [k, v] of Object.entries(query)) {
                if (v !== undefined && v !== null) {
                    url.searchParams.append(k, String(v));
                }
            }
        }

        const transport = url.protocol === 'http:' ? http : https;
        const options = {
            hostname: url.hostname,
            port: url.port || (url.protocol === 'http:' ? 80 : 443),
            path: url.pathname + url.search,
            method,
            headers: {
                'X-Api-Key': API_KEY,
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'User-Agent': `${SERVER_NAME}-mcp/${SERVER_VERSION}`
            },
            timeout: 60000
        };

        const req = transport.request(options, (res) => {
            let data = '';
            res.on('data', (chunk) => { data += chunk; });
            res.on('end', () => {
                let parsed;
                try {
                    parsed = JSON.parse(data);
                } catch {
                    parsed = { raw: data, statusCode: res.statusCode };
                }
                if (res.statusCode >= 400) {
                    parsed._httpStatus = res.statusCode;
                    reject(new Error(`HTTP ${res.statusCode}: ${parsed.message || data || '(sin mensaje)'}`));
                } else {
                    resolve(parsed);
                }
            });
        });

        req.on('error', (err) => reject(new Error(`Error de red: ${err.message}`)));
        req.on('timeout', () => { req.destroy(new Error('Timeout del bridge (>60s)')); });

        if (body) {
            req.write(JSON.stringify(body));
        }
        req.end();
    });
}

// =====================================================================
// DEFINICIÓN DE TOOLS
// Cada tool es una "acción" que el LLM puede invocar.
// =====================================================================

const TOOLS = [
    // -----------------------------------------------------------------
    // STATUS
    // -----------------------------------------------------------------
    {
        name: 'contpaqi_health_check',
        description: 'Comprueba que el bridge está vivo y respondiendo. No requiere auth.',
        inputSchema: { type: 'object', properties: {}, required: [] },
        handler: async () => callBridge('GET', '/api/Status/health')
    },
    {
        name: 'contpaqi_status',
        description: 'Inicializa el SDK de CONTPAQi y devuelve el estado de conexión.',
        inputSchema: { type: 'object', properties: {}, required: [] },
        handler: async () => callBridge('GET', '/api/Status')
    },
    {
        name: 'contpaqi_list_empresas',
        description: 'Lista las carpetas de empresas CONTPAQi disponibles. Útil para que el LLM sepa qué rutaEmpresa usar.',
        inputSchema: { type: 'object', properties: {}, required: [] },
        handler: async () => callBridge('GET', '/api/Empresas')
    },

    // -----------------------------------------------------------------
    // CLIENTES
    // -----------------------------------------------------------------
    {
        name: 'contpaqi_buscar_cliente',
        description: 'Busca un cliente en CONTPAQi por su código. Devuelve null si no existe.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string', description: 'Ruta completa de la empresa, ej: "C:\\\\Compac\\\\Empresas\\\\adMI_EMPRESA"' },
                codigo: { type: 'string', description: 'Código del cliente en CONTPAQi' }
            },
            required: ['rutaEmpresa', 'codigo']
        },
        handler: async ({ rutaEmpresa, codigo }) =>
            callBridge('GET', `/api/Clientes/${encodeURIComponent(codigo)}`, null, { rutaEmpresa })
    },
    {
        name: 'contpaqi_crear_cliente',
        description: 'Crea o actualiza un cliente en CONTPAQi. Si el código ya existe, actualiza sus datos.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                codigo: { type: 'string', description: 'Código único del cliente' },
                razonSocial: { type: 'string', description: 'Nombre o razón social' },
                rfc: { type: 'string', description: 'RFC (12-13 caracteres). Para público en general: XAXX010101000' },
                email: { type: 'string' },
                calle: { type: 'string' },
                colonia: { type: 'string' },
                codigoPostal: { type: 'string', description: '5 dígitos' },
                ciudad: { type: 'string' },
                estado: { type: 'string' },
                pais: { type: 'string', default: 'México' },
                regimenFiscal: { type: 'string', description: 'Clave SAT: 601, 603, 605, 606, 612, 616, 621, etc.' },
                usoCFDI: { type: 'string', description: 'Clave SAT: G01, G02, G03, S01, CP01, etc.' },
                formaPago: { type: 'string', description: '01=Efectivo, 03=Transferencia, 99=Por definir' }
            },
            required: ['rutaEmpresa', 'codigo', 'razonSocial']
        },
        handler: async (params) => callBridge('POST', '/api/Clientes', params)
    },

    // -----------------------------------------------------------------
    // PRODUCTOS
    // -----------------------------------------------------------------
    {
        name: 'contpaqi_buscar_producto',
        description: 'Busca un producto en CONTPAQi por su código. Devuelve null si no existe.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                codigo: { type: 'string' }
            },
            required: ['rutaEmpresa', 'codigo']
        },
        handler: async ({ rutaEmpresa, codigo }) =>
            callBridge('GET', `/api/Productos/${encodeURIComponent(codigo)}`, null, { rutaEmpresa })
    },
    {
        name: 'contpaqi_crear_producto',
        description: 'Crea o actualiza un producto en CONTPAQi.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                codigo: { type: 'string' },
                nombre: { type: 'string' },
                descripcion: { type: 'string' },
                precio: { type: 'number', description: 'Precio unitario en MXN' },
                tipoProducto: { type: 'integer', description: '1=Producto, 2=Paquete, 3=Servicio', default: 1 },
                unidadMedida: { type: 'string', description: 'Clave SAT unidad: H87=Pieza, E48=Servicio, KGM=Kilo, etc.' },
                claveSAT: { type: 'string', description: 'Clave SAT del producto/servicio (8 dígitos)' }
            },
            required: ['rutaEmpresa', 'codigo', 'nombre']
        },
        handler: async (params) => callBridge('POST', '/api/Productos', params)
    },

    // -----------------------------------------------------------------
    // FACTURAS
    // -----------------------------------------------------------------
    {
        name: 'contpaqi_crear_factura',
        description: 'Crea una factura (sin timbrar). Si el cliente o producto no existen y se pasan datos extra, los crea automáticamente.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                codigoConcepto: { type: 'string', description: 'Típicamente "4" para factura' },
                codigoCliente: { type: 'string' },
                clienteRazonSocial: { type: 'string', description: 'Opcional: si el cliente no existe, se crea con estos datos' },
                clienteRFC: { type: 'string' },
                clienteRegimenFiscal: { type: 'string' },
                usoCFDI: { type: 'string', default: 'G01' },
                formaPago: { type: 'string', default: '99' },
                metodoPago: { type: 'string', description: 'PUE o PPD', default: 'PUE' },
                productos: {
                    type: 'array',
                    items: {
                        type: 'object',
                        properties: {
                            codigo: { type: 'string' },
                            nombre: { type: 'string' },
                            cantidad: { type: 'number' },
                            precio: { type: 'number' },
                            unidadMedida: { type: 'string' },
                            claveSAT: { type: 'string' }
                        },
                        required: ['codigo', 'cantidad', 'precio']
                    }
                }
            },
            required: ['rutaEmpresa', 'codigoConcepto', 'codigoCliente', 'productos']
        },
        handler: async (params) => callBridge('POST', '/api/Documentos/factura', params)
    },
    {
        name: 'contpaqi_timbrar_factura',
        description: 'Timbra una factura existente ante el SAT. Requiere los archivos CSD (.cer/.key) en la carpeta de la empresa.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                codigoConcepto: { type: 'string', description: 'Típicamente "4"' },
                serie: { type: 'string', default: '' },
                folio: { type: 'number' },
                passCSD: { type: 'string', description: 'Contraseña del certificado CSD' }
            },
            required: ['rutaEmpresa', 'codigoConcepto', 'folio', 'passCSD']
        },
        handler: async (params) => callBridge('POST', '/api/Documentos/timbrar', params)
    },
    {
        name: 'contpaqi_validar_factura',
        description: 'Valida una factura antes de timbrarla (sin enviar al SAT). Detecta errores de catálogo, CSD, CFDI 4.0.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                codigoConcepto: { type: 'string' },
                serie: { type: 'string', default: '' },
                folio: { type: 'number' },
                passCSD: { type: 'string' }
            },
            required: ['rutaEmpresa', 'codigoConcepto', 'folio']
        },
        handler: async (params) => callBridge('POST', '/api/Documentos/validar', params)
    },
    {
        name: 'contpaqi_obtener_xml',
        description: 'Descarga el XML (o PDF) del CFDI timbrado.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                codigoConcepto: { type: 'string' },
                serie: { type: 'string', default: '' },
                folio: { type: 'number' },
                formato: { type: 'integer', description: '0=XML, 1=PDF', default: 0 }
            },
            required: ['rutaEmpresa', 'codigoConcepto', 'folio']
        },
        handler: async ({ rutaEmpresa, codigoConcepto, serie, folio, formato }) =>
            callBridge('GET', '/api/Documentos/xml', null, { rutaEmpresa, codigoConcepto, serie, folio, formato: formato || 0 })
    },
    {
        name: 'contpaqi_obtener_uuid',
        description: 'Obtiene el UUID (folio fiscal) de un CFDI timbrado.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                codigoConcepto: { type: 'string' },
                serie: { type: 'string', default: '' },
                folio: { type: 'number' }
            },
            required: ['rutaEmpresa', 'codigoConcepto', 'folio']
        },
        handler: async ({ rutaEmpresa, codigoConcepto, serie, folio }) =>
            callBridge('GET', '/api/Documentos/uuid', null, { rutaEmpresa, codigoConcepto, serie, folio })
    },
    {
        name: 'contpaqi_cancelar_factura',
        description: 'Cancela un CFDI 4.0 ante el SAT. Motivos: 01=Con relación (requiere uuidSustitucion), 02=Sin relación, 03=No realizada, 04=Factura global.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                codigoConcepto: { type: 'string' },
                serie: { type: 'string', default: '' },
                folio: { type: 'number' },
                motivoCancelacion: { type: 'string', description: '01, 02, 03 o 04', default: '02' },
                passCSD: { type: 'string' },
                uuidSustitucion: { type: 'string', description: 'Requerido solo si motivoCancelacion=01' }
            },
            required: ['rutaEmpresa', 'codigoConcepto', 'folio']
        },
        handler: async (params) => callBridge('POST', '/api/Documentos/cancelar', params)
    },
    {
        name: 'contpaqi_flujo_completo',
        description: '⭐ ENDPOINT ESTRELLA: crea cliente + producto + factura + timbra en una sola llamada. Si passCSD viene, se timbra automáticamente.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                cliente: {
                    type: 'object',
                    properties: {
                        codigo: { type: 'string' },
                        razonSocial: { type: 'string' },
                        rfc: { type: 'string' },
                        regimenFiscal: { type: 'string' },
                        usoCFDI: { type: 'string' }
                    },
                    required: ['codigo', 'razonSocial']
                },
                producto: {
                    type: 'object',
                    properties: {
                        codigo: { type: 'string' },
                        nombre: { type: 'string' },
                        precio: { type: 'number' },
                        claveSAT: { type: 'string' },
                        unidadMedida: { type: 'string' }
                    },
                    required: ['codigo', 'nombre', 'precio']
                },
                factura: {
                    type: 'object',
                    properties: {
                        codigoConcepto: { type: 'string', default: '4' },
                        cantidad: { type: 'number', default: 1 },
                        passCSD: { type: 'string' },
                        usoCFDI: { type: 'string' },
                        formaPago: { type: 'string' },
                        metodoPago: { type: 'string' }
                    }
                }
            },
            required: ['rutaEmpresa', 'cliente', 'producto', 'factura']
        },
        handler: async (params) => callBridge('POST', '/api/Integracion/flujo-completo', params)
    },

    // -----------------------------------------------------------------
    // SINCRONIZACIÓN
    // -----------------------------------------------------------------
    {
        name: 'contpaqi_pull_clientes',
        description: 'Sincroniza clientes desde CONTPAQi hacia tu sistema (snapshot completo). Para sync incremental usa contpaqi_pull_clientes_modificados.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                limite: { type: 'integer', default: 500 }
            },
            required: ['rutaEmpresa']
        },
        handler: async ({ rutaEmpresa, limite }) =>
            callBridge('GET', '/api/Sync/clientes', null, { rutaEmpresa, limite: limite || 500 })
    },
    {
        name: 'contpaqi_pull_clientes_modificados',
        description: 'Sincronización incremental: solo clientes cambiados desde una fecha.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                desde: { type: 'string', description: 'Fecha ISO 8601: 2024-01-01T00:00:00' },
                limite: { type: 'integer', default: 500 }
            },
            required: ['rutaEmpresa', 'desde']
        },
        handler: async ({ rutaEmpresa, desde, limite }) =>
            callBridge('GET', '/api/Sync/clientes/modificados', null, { rutaEmpresa, desde, limite: limite || 500 })
    },
    {
        name: 'contpaqi_pull_productos',
        description: 'Snapshot completo de productos desde CONTPAQi.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                limite: { type: 'integer', default: 500 }
            },
            required: ['rutaEmpresa']
        },
        handler: async ({ rutaEmpresa, limite }) =>
            callBridge('GET', '/api/Sync/productos', null, { rutaEmpresa, limite: limite || 500 })
    },
    {
        name: 'contpaqi_pull_productos_modificados',
        description: 'Sincronización incremental de productos.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                desde: { type: 'string' },
                limite: { type: 'integer', default: 500 }
            },
            required: ['rutaEmpresa', 'desde']
        },
        handler: async ({ rutaEmpresa, desde, limite }) =>
            callBridge('GET', '/api/Sync/productos/modificados', null, { rutaEmpresa, desde, limite: limite || 500 })
    },
    {
        name: 'contpaqi_pull_facturas',
        description: 'Trae las facturas/documentos modificados en CONTPAQi desde una fecha. Cada factura trae UUID, folio, total, cliente, etc.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                desde: { type: 'string' },
                limite: { type: 'integer', default: 500 }
            },
            required: ['rutaEmpresa', 'desde']
        },
        handler: async ({ rutaEmpresa, desde, limite }) =>
            callBridge('GET', '/api/Sync/documentos/modificados', null, { rutaEmpresa, desde, limite: limite || 500 })
    },
    {
        name: 'contpaqi_push_clientes_batch',
        description: 'Crea/actualiza un lote de clientes en CONTPAQi (push masivo desde tu sistema).',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                items: {
                    type: 'array',
                    items: {
                        type: 'object',
                        properties: {
                            codigo: { type: 'string' },
                            razonSocial: { type: 'string' },
                            rfc: { type: 'string' },
                            email: { type: 'string' },
                            regimenFiscal: { type: 'string' },
                            usoCFDI: { type: 'string' },
                            formaPago: { type: 'string' }
                        },
                        required: ['codigo', 'razonSocial']
                    }
                }
            },
            required: ['rutaEmpresa', 'items']
        },
        handler: async (params) => callBridge('POST', '/api/Sync/clientes/batch', params)
    },
    {
        name: 'contpaqi_push_productos_batch',
        description: 'Crea/actualiza un lote de productos en CONTPAQi.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                items: {
                    type: 'array',
                    items: {
                        type: 'object',
                        properties: {
                            codigo: { type: 'string' },
                            nombre: { type: 'string' },
                            precio: { type: 'number' },
                            claveSAT: { type: 'string' },
                            unidadMedida: { type: 'string' }
                        },
                        required: ['codigo', 'nombre', 'precio']
                    }
                }
            },
            required: ['rutaEmpresa', 'items']
        },
        handler: async (params) => callBridge('POST', '/api/Sync/productos/batch', params)
    },

    // -----------------------------------------------------------------
    // REPORTES
    // -----------------------------------------------------------------
    {
        name: 'contpaqi_reporte_ventas',
        description: 'Reporte de ventas por día en un periodo.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                desde: { type: 'string', description: 'Fecha YYYY-MM-DD' },
                hasta: { type: 'string', description: 'Fecha YYYY-MM-DD' }
            },
            required: ['rutaEmpresa', 'desde', 'hasta']
        },
        handler: async ({ rutaEmpresa, desde, hasta }) =>
            callBridge('GET', '/api/Reportes/ventas', null, { rutaEmpresa, desde, hasta })
    },
    {
        name: 'contpaqi_reporte_top_clientes',
        description: 'Top N clientes por ventas en un periodo.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                desde: { type: 'string' },
                hasta: { type: 'string' },
                top: { type: 'integer', default: 10 }
            },
            required: ['rutaEmpresa', 'desde', 'hasta']
        },
        handler: async ({ rutaEmpresa, desde, hasta, top }) =>
            callBridge('GET', '/api/Reportes/top-clientes', null, { rutaEmpresa, desde, hasta, top: top || 10 })
    },
    {
        name: 'contpaqi_reporte_top_productos',
        description: 'Top N productos más vendidos en un periodo.',
        inputSchema: {
            type: 'object',
            properties: {
                rutaEmpresa: { type: 'string' },
                desde: { type: 'string' },
                hasta: { type: 'string' },
                top: { type: 'integer', default: 10 }
            },
            required: ['rutaEmpresa', 'desde', 'hasta']
        },
        handler: async ({ rutaEmpresa, desde, hasta, top }) =>
            callBridge('GET', '/api/Reportes/top-productos', null, { rutaEmpresa, desde, hasta, top: top || 10 })
    },

    // -----------------------------------------------------------------
    // WEBHOOKS
    // -----------------------------------------------------------------
    {
        name: 'contpaqi_registrar_webhook',
        description: 'Registra un webhook para recibir notificaciones de eventos (timbrado, cancelación, etc.).',
        inputSchema: {
            type: 'object',
            properties: {
                evento: {
                    type: 'string',
                    description: 'timbrado.exitoso, timbrado.fallido, cancelacion.exitosa, cancelacion.fallida, documento.creado, o "*" para todos'
                },
                url: { type: 'string', description: 'URL completa que recibirá el POST con el payload del evento' }
            },
            required: ['evento', 'url']
        },
        handler: async (params) => callBridge('POST', '/api/Webhooks', params)
    },
    {
        name: 'contpaqi_listar_webhooks',
        description: 'Lista todos los webhooks registrados.',
        inputSchema: { type: 'object', properties: {}, required: [] },
        handler: async () => callBridge('GET', '/api/Webhooks')
    }
];

// =====================================================================
// CARGA DEL MANUAL DEL SDK (recursos MCP)
// =====================================================================
//
// El manual completo del SDK de CONTPAQi está disponible como recursos MCP
// (scheme "manual://") para que el LLM pueda consultarlo on-demand sin
// volver a pedirlo. Fuente original:
// https://conocimiento.blob.core.windows.net/conocimiento/Manuales/MR_SDK/

const MANUAL_DIR = path.join(__dirname, 'manual_md');
const MANUAL_INDEX = path.join(MANUAL_DIR, 'index.json');

let MANUAL_ENTRIES = [];     // [{ uri, name, file, size, source, mimeType, description }]
let MANUAL_BY_URI = new Map();
let MANUAL_LOAD_ERROR = null;

function loadManualIndex() {
    try {
        if (!fs.existsSync(MANUAL_INDEX)) {
            MANUAL_LOAD_ERROR = `No se encontró ${MANUAL_INDEX}. Ejecuta convert_manual.js primero.`;
            return;
        }
        const raw = fs.readFileSync(MANUAL_INDEX, 'utf8');
        const arr = JSON.parse(raw);
        MANUAL_ENTRIES = arr.map((e) => ({
            uri: e.uri,
            name: e.name,
            file: e.file,
            size: e.size,
            source: e.source,
            mimeType: 'text/markdown',
            description: `Manual de Referencia del SDK CONTPAQi - ${e.name}`
        }));
        MANUAL_BY_URI = new Map(MANUAL_ENTRIES.map((e) => [e.uri, e]));
        process.stderr.write(`[${SERVER_NAME}] Manual cargado: ${MANUAL_ENTRIES.length} capítulos.\n`);
    } catch (err) {
        MANUAL_LOAD_ERROR = `Error cargando manual: ${err.message}`;
        process.stderr.write(`[${SERVER_NAME}] ${MANUAL_LOAD_ERROR}\n`);
    }
}

loadManualIndex();

function readManualChapter(uri) {
    const entry = MANUAL_BY_URI.get(uri);
    if (!entry) return null;
    const filePath = path.join(MANUAL_DIR, entry.file);
    if (!fs.existsSync(filePath)) return null;
    try {
        return {
            uri: entry.uri,
            mimeType: 'text/markdown',
            text: fs.readFileSync(filePath, 'utf8')
        };
    } catch (err) {
        return null;
    }
}

function searchManual(query, maxResults = 10) {
    if (!query || !MANUAL_ENTRIES.length) return [];
    const q = query.toLowerCase();
    const results = [];
    for (const entry of MANUAL_ENTRIES) {
        const filePath = path.join(MANUAL_DIR, entry.file);
        if (!fs.existsSync(filePath)) continue;
        try {
            const text = fs.readFileSync(filePath, 'utf8').toLowerCase();
            const idx = text.indexOf(q);
            if (idx >= 0) {
                // Calcular score simple por número de ocurrencias
                let count = 0;
                let pos = 0;
                while ((pos = text.indexOf(q, pos)) !== -1) {
                    count++;
                    pos += q.length;
                    if (count > 50) break; // límite para evitar búsquedas lentas
                }
                results.push({
                    uri: entry.uri,
                    name: entry.name,
                    file: entry.file,
                    snippet: text.substring(Math.max(0, idx - 60), idx + q.length + 100)
                        .replace(/\s+/g, ' ')
                        .trim(),
                    score: count
                });
            }
        } catch {}
    }
    results.sort((a, b) => b.score - a.score);
    return results.slice(0, maxResults);
}

// =====================================================================
// TOOLS DEL MANUAL (backup para clientes sin soporte de resources)
// =====================================================================

const MANUAL_TOOLS = [
    {
        name: 'contpaqi_sdk_manual_list',
        description: 'Lista los capítulos del Manual de Referencia del SDK de CONTPAQi. Devuelve ~70 capítulos con título, URI y tamaño. Usar cuando el LLM necesita saber qué información está disponible antes de consultarla.',
        inputSchema: {
            type: 'object',
            properties: {
                filtro: { type: 'string', description: 'Texto opcional para filtrar capítulos por título (case-insensitive). Ej: "timbrado", "cliente", "documento".' }
            },
            required: []
        },
        handler: async ({ filtro } = {}) => {
            if (!MANUAL_ENTRIES.length) {
                return { error: MANUAL_LOAD_ERROR || 'Manual no disponible' };
            }
            let entries = MANUAL_ENTRIES;
            if (filtro) {
                const f = filtro.toLowerCase();
                entries = entries.filter((e) => e.name.toLowerCase().includes(f));
            }
            return {
                total: entries.length,
                totalGeneral: MANUAL_ENTRIES.length,
                capitulos: entries.map((e) => ({
                    uri: e.uri,
                    titulo: e.name,
                    tamaño: e.size
                }))
            };
        }
    },
    {
        name: 'contpaqi_sdk_manual_get',
        description: 'Obtiene el contenido completo (Markdown) de un capítulo del Manual de Referencia del SDK de CONTPAQi. Use el URI devuelto por contpaqi_sdk_manual_list.',
        inputSchema: {
            type: 'object',
            properties: {
                uri: { type: 'string', description: 'URI del capítulo, formato "manual://<slug>". Ej: "manual://introduccion"' },
                slug: { type: 'string', description: 'Alternativa al uri: nombre del archivo sin extensión. Ej: "introduccion", "funciones_de_documentos"' }
            },
            required: []
        },
        handler: async ({ uri, slug } = {}) => {
            if (!MANUAL_ENTRIES.length) {
                return { error: MANUAL_LOAD_ERROR || 'Manual no disponible' };
            }
            let targetUri = uri;
            if (!targetUri && slug) {
                targetUri = 'manual://' + slug.replace(/^manual:\/\//, '');
            }
            if (!targetUri) {
                return { error: 'Especifica uri o slug' };
            }
            const chapter = readManualChapter(targetUri);
            if (!chapter) {
                return { error: `Capítulo no encontrado: ${targetUri}` };
            }
            const entry = MANUAL_BY_URI.get(targetUri);
            return {
                uri: chapter.uri,
                titulo: entry.name,
                fuente: entry.source,
                markdown: chapter.text
            };
        }
    },
    {
        name: 'contpaqi_sdk_manual_search',
        description: 'Busca texto en todo el Manual de Referencia del SDK de CONTPAQi. Devuelve los capítulos que contienen el término con un snippet del contexto. Útil para localizar funciones o conceptos específicos.',
        inputSchema: {
            type: 'object',
            properties: {
                query: { type: 'string', description: 'Texto a buscar. Ej: "fAltaDocumento", "timbrado", "UUID"' },
                limite: { type: 'integer', description: 'Máximo de resultados', default: 5 }
            },
            required: ['query']
        },
        handler: async ({ query, limite } = {}) => {
            if (!query) return { error: 'query requerido' };
            const results = searchManual(query, limite || 5);
            return {
                query,
                total: results.length,
                resultados: results.map((r) => ({
                    uri: r.uri,
                    titulo: r.name,
                    snippet: '...' + r.snippet + '...',
                    ocurrencias: r.score
                }))
            };
        }
    },
    {
        name: 'contpaqi_sdk_manual_overview',
        description: 'Devuelve un índice compacto del manual con todas las URIs y títulos. Útil para inyectar en contexto cuando el LLM necesita conocer la estructura completa.',
        inputSchema: { type: 'object', properties: {}, required: [] },
        handler: async () => {
            if (!MANUAL_ENTRIES.length) {
                return { error: MANUAL_LOAD_ERROR || 'Manual no disponible' };
            }
            // Genera un índice tipo tabla de contenidos
            const lines = MANUAL_ENTRIES.map((e, i) =>
                `${String(i + 1).padStart(2, '0')}. [${e.name}](${e.uri}) (${(e.size / 1024).toFixed(1)} KB)`
            );
            return {
                total: MANUAL_ENTRIES.length,
                indice: lines.join('\n')
            };
        }
    }
];

// =====================================================================
// SERVIDOR MCP (JSON-RPC 2.0 sobre stdio)
// =====================================================================

const ALL_TOOLS = [...TOOLS, ...MANUAL_TOOLS];
const TOOL_MAP = new Map(ALL_TOOLS.map((t) => [t.name, t]));

function sendMessage(msg) {
    const json = JSON.stringify(msg);
    // Usar write con callback para garantizar envío
    process.stdout.write(json + '\n', (err) => {
        if (err) process.stderr.write(`[${SERVER_NAME}] Error escribiendo a stdout: ${err.message}\n`);
    });
}

function sendError(id, code, message, data) {
    sendMessage({
        jsonrpc: '2.0',
        id,
        error: { code, message, data }
    });
}

function sendResult(id, result) {
    sendMessage({
        jsonrpc: '2.0',
        id,
        result
    });
}

async function handleRequest(req) {
    const { id, method, params } = req;

    try {
        switch (method) {
            case 'initialize':
                sendResult(id, {
                    protocolVersion: '2024-11-05',
                    serverInfo: { name: SERVER_NAME, version: SERVER_VERSION },
                    capabilities: {
                        tools: {},
                        resources: MANUAL_ENTRIES.length > 0 ? {} : undefined
                    }
                });
                break;

            case 'notifications/initialized':
                // Cliente notifica que terminó inicialización. No respondemos.
                break;

            case 'tools/list': {
                const tools = ALL_TOOLS.map(({ name, description, inputSchema }) => ({
                    name, description, inputSchema
                }));
                sendResult(id, { tools });
                break;
            }

            case 'tools/call': {
                const { name, arguments: args = {} } = params || {};
                const tool = TOOL_MAP.get(name);
                if (!tool) {
                    sendError(id, -32602, `Tool no encontrada: ${name}`);
                    return;
                }

                try {
                    const result = await tool.handler(args || {});
                    sendResult(id, {
                        content: [
                            {
                                type: 'text',
                                text: typeof result === 'string' ? result : JSON.stringify(result, null, 2)
                            }
                        ],
                        isError: false
                    });
                } catch (err) {
                    sendResult(id, {
                        content: [
                            {
                                type: 'text',
                                text: `Error ejecutando ${name}: ${err.message}`
                            }
                        ],
                        isError: true
                    });
                }
                break;
            }

            case 'resources/list': {
                if (!MANUAL_ENTRIES.length) {
                    sendResult(id, { resources: [], _error: MANUAL_LOAD_ERROR || 'Manual no disponible' });
                    return;
                }
                const resources = MANUAL_ENTRIES.map((e) => ({
                    uri: e.uri,
                    name: e.name,
                    description: e.description,
                    mimeType: e.mimeType,
                    size: e.size
                }));
                sendResult(id, { resources });
                break;
            }

            case 'resources/templates/list': {
                sendResult(id, {
                    resourceTemplates: [
                        {
                            uriTemplate: 'manual://{slug}',
                            name: 'Capítulo del Manual SDK CONTPAQi',
                            description: 'Accede a cualquier capítulo del Manual de Referencia del SDK por su slug (nombre del archivo sin extensión). Ej: manual://introduccion, manual://funciones_de_documentos, manual://tipos_de_datos_abstractos_del_sdk',
                            mimeType: 'text/markdown'
                        }
                    ]
                });
                break;
            }

            case 'resources/read': {
                const { uri } = params || {};
                if (!uri) {
                    sendError(id, -32602, 'Falta uri');
                    return;
                }
                const chapter = readManualChapter(uri);
                if (!chapter) {
                    sendError(id, -32002, `Recurso no encontrado: ${uri}`);
                    return;
                }
                sendResult(id, {
                    contents: [
                        {
                            uri: chapter.uri,
                            mimeType: chapter.mimeType,
                            text: chapter.text
                        }
                    ]
                });
                break;
            }

            case 'ping':
                sendResult(id, {});
                break;

            default:
                sendError(id, -32601, `Método no implementado: ${method}`);
        }
    } catch (err) {
        sendError(id, -32603, `Error interno: ${err.message}`);
    }
}

// =====================================================================
// LOOP PRINCIPAL: leer líneas de stdin (delimitadas por \n)
// =====================================================================

process.stderr.write(`[${SERVER_NAME}] MCP server v${SERVER_VERSION} listo. Bridge: ${BRIDGE_URL}\n`);

let buffer = '';
let pendingRequests = 0;
let stdinClosed = false;

function maybeExit() {
    if (stdinClosed && pendingRequests === 0) {
        if (process.stdout.write('')) {
            process.exit(0);
        } else {
            process.stdout.once('drain', () => process.exit(0));
        }
    }
}

process.stdin.setEncoding('utf8');
process.stdin.on('data', (chunk) => {
    buffer += chunk;
    let newlineIdx;
    while ((newlineIdx = buffer.indexOf('\n')) !== -1) {
        const line = buffer.slice(0, newlineIdx).trim();
        buffer = buffer.slice(newlineIdx + 1);
        if (!line) continue;

        let req;
        try {
            req = JSON.parse(line);
        } catch (err) {
            sendError(null, -32700, `Parse error: ${err.message}`);
            continue;
        }

        pendingRequests++;
        handleRequest(req).finally(() => {
            pendingRequests--;
            setImmediate(maybeExit);
        });
    }
});

process.stdin.on('end', () => {
    process.stderr.write(`[${SERVER_NAME}] stdin cerrado.\n`);
    stdinClosed = true;
    maybeExit();
});

// Manejo de errores no capturados
process.on('uncaughtException', (err) => {
    process.stderr.write(`[${SERVER_NAME}] Uncaught: ${err.stack || err.message}\n`);
});
process.on('unhandledRejection', (err) => {
    process.stderr.write(`[${SERVER_NAME}] Unhandled rejection: ${err.stack || err}\n`);
});