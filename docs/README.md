# ContpaqiBridge — Documentación

API REST en .NET 6 que expone el SDK de CONTPAQi Comercial Premium a sistemas externos a través de HTTP. Es un reemplazo directo del servicio comercial "AR Software - CONTPAQi Comercial API" pero auto-hospedado y sin costo por RFC.

## 📚 Índice de documentación

| Documento | Contenido |
|---|---|
| [README.md](../README.md) | Inicio rápido, instalación, ejemplos |
| [API.md](./API.md) | **Referencia completa de endpoints** con todos los parámetros |
| [ARCHITECTURE.md](./ARCHITECTURE.md) | Arquitectura interna, componentes, P/Invoke, threading |
| [DEPLOYMENT.md](./DEPLOYMENT.md) | Despliegue en producción: Windows VPS, IIS, ZeroTier, ngrok |
| [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) | Problemas comunes y soluciones |
| [Laravel-Examples/](../Laravel-Examples/README.md) | Integración con Laravel (cliente HTTP, sync, webhooks) |
| [mcp-server/](../mcp-server/README.md) | Servidor MCP para usar el bridge desde opencode/Claude |

## 🎯 ¿Qué problema resuelve?

CONTPAQi Comercial Premium solo se puede automatizar mediante su **SDK COM local** (que requiere Windows x86 + licencia de CONTPAQi). Esto significa que:

- ❌ No puedes facturar desde un VPS Linux
- ❌ No puedes automatizar desde la nube
- ❌ No puedes exponerlo como API REST a tus clientes

**ContpaqiBridge** resuelve esto: se instala en la misma máquina Windows donde está CONTPAQi y expone toda su funcionalidad a través de HTTP. Tus aplicaciones en la nube (Laravel, Node, Python, etc.) consumen el bridge como cualquier API REST.

## 🧩 Componentes principales

```
┌─────────────────────────────────────────────────────────────────┐
│ ContpaqiBridge (.NET 6 x86)                                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Program.cs              ← ASP.NET Core pipeline                │
│  ├── ApiKeyMiddleware     ← Autenticación X-Api-Key             │
│  ├── CorsMiddleware       ← CORS para frontends                 │
│  └── Controllers/                                               │
│      ├── StatusController      ← Health, init, conexión         │
│      ├── ClientesController    ← CRUD clientes                  │
│      ├── ProductosController   ← CRUD productos                 │
│      ├── DocumentosController  ← Facturas, timbrado, XML        │
│      ├── IntegracionController ← Flujo completo en 1 llamada    │
│      ├── SyncController        ← Sync bidireccional              │
│      ├── ReportesController    ← Reportes SQL                   │
│      ├── WebhooksController    ← Sistema de webhooks            │
│      ├── EmpresasController    ← Listar empresas disponibles    │
│      └── DocsController        ← Docs HTML + OpenAPI            │
│                                                                 │
│  Services/ContpaqiSdkService.cs ← Lógica SDK (P/Invoke)         │
│  ├── Inicializa/Abre/Cierra empresa                              │
│  ├── P/Invoke a MGWServicios.dll (32-bit)                       │
│  ├── Lectura directa SQL Server (catálogos, sync)               │
│  ├── Sistema de webhooks en memoria                             │
│  └── Thread-safe con lock interno                               │
│                                                                 │
│  Middleware/ApiKeyMiddleware.cs ← Auth                          │
│  Models/                       ← DTOs request                  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
                          │
                          │ HTTP REST
                          ▼
              ┌──────────────────────┐
              │  Clientes HTTP       │
              │  (Laravel, Node, etc)│
              └──────────────────────┘
```

## 🔐 Seguridad

- **Autenticación por API Key** (header `X-Api-Key` o query `?api_key=`).
- Comparación timing-safe para evitar timing attacks.
- CORS abierto por defecto (cambiar si es para producción pública).
- HTTPS no incluido: usar reverse proxy (IIS, nginx, Caddy) en producción.
- Endpoints públicos solo: `/api/Status/health`, `/api/Docs/*`.

## 🚦 Estado del proyecto

- ✅ Build limpio (0 errores, 0 warnings críticos)
- ✅ Implementados: 28+ endpoints
- ✅ Sincronización bidireccional Laravel ↔ CONTPAQi
- ✅ Sistema de webhooks
- ✅ Reportes SQL
- ✅ MCP server para opencode/Claude
- ✅ Ejemplos Laravel completos

## 📊 Comparación con AR Software

| Característica | AR Software | ContpaqiBridge |
|---|---|---|
| Costo anual | $720 USD/RFC | Gratis |
| Auto-hospedado | No (su servidor) | Sí (tu máquina) |
| Código fuente | No | Sí (este repo) |
| Personalizable | No | Totalmente |
| Multi-empresa | Sí | Sí |
| Webhooks | No documentado | Sí |
| Sync bidireccional | No documentado | Sí |
| Soporte | Limitado | Tú mismo |
| Latencia | Internet → AR → CONTPAQi | Directo (red local) |
| Dependencia externa | Sí | No |

## 📝 Licencia

MIT — úsalo, modifícalo, véndelo. Sin restricciones.