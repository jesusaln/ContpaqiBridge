# Laravel ↔ ContpaqiBridge — Guía de integración

Esta carpeta contiene los archivos PHP que necesitas para conectar tu Laravel al bridge de CONTPAQi.

## ✅ Ya están en el bridge (no necesitas hacer nada)

| Feature | Implementación | Archivo |
|---------|----------------|---------|
| Webhook con HMAC-SHA256 | `ComputeHmac()` + header `X-Contpaqi-Signature` | `Services/WebhookService.cs` |
| Retry con backoff | 0s, 60s, 5min, 15min, persistente en disco | `EnviarConRetry()` en `WebhookService.cs` |
| Sync log | `data/sync-log.json` + `GET /api/Sync/log` | `LogEvent()` en `WebhookService.cs` |

## 🌐 Endpoint público

**`https://bridge.asistenciavircom.com`** (Nginx + Let's Encrypt + Cloudflare proxy)

| URL | Método | Auth |
|-----|--------|------|
| `https://bridge.asistenciavircom.com/health` | GET | ❌ público |
| `https://bridge.asistenciavircom.com/api/Status` | GET | ✅ |
| `https://bridge.asistenciavircom.com/api/Documentos/factura` | POST | ✅ |
| `https://bridge.asistenciavircom.com/api/Documentos/timbrar` | POST | ✅ |
| `https://bridge.asistenciavircom.com/api/Documentos/validar` | POST | ✅ |
| `https://bridge.asistenciavircom.com/api/Sync/clientes` | GET | ✅ |
| `https://bridge.asistenciavircom.com/api/Sync/productos` | GET | ✅ |
| `https://bridge.asistenciavircom.com/webhooks/contpaqi` | POST | ❌* |

*Sin auth, pero valida HMAC en el body.

Header requerido para endpoints autenticados: `X-Api-Key: <clave>`.

## 📁 Archivos para Laravel

| Archivo | Ubicación | Propósito |
|---------|-----------|-----------|
| `ContpaqiBridgeClient.php` | `app/Services/` | Cliente HTTP con retry para consumir sync |
| `ContpaqiFacturadorService.php` | `app/Services/` | **NUEVO** — Crear + timbrar facturas vía bridge |
| `ContpaqiWebhookController.php` | `app/Http/Controllers/` | Recibe webhooks firmados |
| `FacturaController.php` | `app/Http/Controllers/Api/` | **NUEVO** — REST endpoint para tu app |
| `ContpaqiSyncCommand.php` | `app/Console/Commands/` | Artisan command para sync |

## 🔐 Configuración (Laravel .env)

```env
CONTPAQI_BRIDGE_URL=https://bridge.asistenciavircom.com
CONTPAQI_BRIDGE_KEY=TU_API_KEY_SECRETA_DE_64_CARACTERES_MINIMO
CONTPAQI_WEBHOOK_SECRET=cualquier_string_largo_aqui_lo_cambiaremos
```

Y `config/contpaqi.php`:
```php
return [
    'base_url'        => env('CONTPAQI_BRIDGE_URL'),
    'api_key'         => env('CONTPAQI_BRIDGE_KEY'),
    'webhook_secret'  => env('CONTPAQI_WEBHOOK_SECRET'),
    'max_attempts'    => 4,
    'empresa_default' => 'adJESUS_LOPEZ_NORIEGA',
];
```

## 🚀 Flujo end-to-end: Laravel crea una factura → bridge timbra → bridge notifica a Laravel

```
┌─────────────────┐  POST /api/facturas   ┌─────────────────┐
│  Tu App Laravel  │ ──────────────────▶ │  FacturaController│
│  (PHP)           │                       │  (Laravel)       │
└─────────────────┘                       └────────┬────────┘
                                                    │
                                                    ▼
                                          ContpaqiFacturadorService
                                          (cliente HTTP al bridge)
                                                    │
                                                    ▼  HTTPS + X-Api-Key
                                          ┌─────────────────────┐
                                          │ bridge.asistenciavi │
                                          │ rcom.com (Nginx)    │
                                          └──────────┬──────────┘
                                                    │
                                                    ▼ proxy_pass ZeroTier
                                          ┌─────────────────────┐
                                          │ Bridge (PC Windows) │
                                          │ - SDK CONTPAQi      │
                                          │ - Crea factura      │
                                          │ - Timbra            │
                                          └──────────┬──────────┘
                                                    │
                                                    ▼
                                          ┌─────────────────────┐
                                          │ CONTPAQi Comercial  │
                                          │ - Crea folio        │
                                          │ - Llama al PAC      │
                                          │ - Emite CFDI        │
                                          │ - Guarda XML/PDF    │
                                          └──────────┬──────────┘
                                                    │
                                                    │ Webhook con HMAC
                                                    ▼
                                          ┌─────────────────────┐
                                          │ /webhooks/contpaqi  │
                                          │ en TU Laravel       │
                                          │ (ContpaqiWebhook-   │
                                          │  Controller)        │
                                          └─────────────────────┘
```

## 📝 Ejemplo: cómo facturar desde Laravel

```php
use App\Services\ContpaqiFacturadorService;

$facturador = app(ContpaqiFacturadorService::class);

// Crear y timbrar en una sola llamada
$resultado = $facturador->crearYTimbrar([
    'ruta_empresa' => 'C:\\Compac\\Empresas\\adJESUS_LOPEZ_NORIEGA',
    'concepto'     => '4CLIMAS',
    'cliente'      => 'CLI003AUT02',
    'productos'    => [
        [
            'codigo'       => 'PROD003AUTO',
            'cantidad'     => 1,
            'precio'       => 250.00,
            'unidad_medida'=> 'H87',
            'clave_sat'    => '01010101',
        ],
    ],
    'cliente_datos' => [
        'razon_social'   => 'CLIENTE AUTO 003',
        'rfc'            => 'AUA030303XYZ',
        'regimen_fiscal' => '601',
    ],
], 'password_del_CSD');

// Resultado:
// {
//   'ok' => true,
//   'mensaje' => 'Factura timbrada correctamente',
//   'id_documento' => 5753,
//   'serie' => 'CDD',
//   'folio' => 413
// }

// Si solo quieres crear (sin timbrar):
$resultado = $facturador->facturar([...]);
```

## 🌐 Rutas REST (routes/api.php)

```php
use App\Http\Controllers\Api\FacturaController;
use App\Http\Controllers\ContpaqiWebhookController;

Route::middleware(['auth:sanctum'])->prefix('api/facturas')->group(function () {
    Route::post('/', [FacturaController::class, 'store']);
    Route::post('/{id}/timbrar', [FacturaController::class, 'timbrar']);
});

Route::post('/webhooks/contpaqi', [ContpaqiWebhookController::class, 'handle']);
```

## 🪝 Webhook receiver (Laravel recibe notificaciones del bridge)

`ContpaqiWebhookController.php` ya está provisto. Para activarlo:

1. **Obtén el secret de cada webhook** que generaste con `POST /api/Webhooks` (campo `secret` en la respuesta).
2. **Guarda los secrets en Laravel** (en `config/contpaqi.php` por ahora; después en BD).
3. **El bridge envía webhooks firmados** con `X-Contpaqi-Signature: sha256=<hmac>`. Laravel verifica el HMAC antes de procesar el payload.

Ejemplo: cuando una factura es timbrada, el bridge llama a `https://bridge.asistenciavircom.com/webhooks/contpaqi` con:
```
X-Contpaqi-Signature: sha256=<hmac del body>
X-Contpaqi-Event: timbrado.exitoso
X-Contpaqi-Delivery: <uuid>
Body: { "evento": "timbrado.exitoso", "timestamp": "...", "payload": {...} }
```

Tu `ContpaqiWebhookController::handle()` verifica la firma con `hash_equals($hmac_calculado, $firma_recibida)` y actualiza la BD local con el UUID, folio timbrado, etc.

## 🔁 Cómo el bridge reintenta si Laravel está caído

El bridge tiene retry automático: si tu Laravel devuelve 500 o no responde, el bridge reintenta 4 veces con backoff exponencial (0s, 60s, 5min, 15min). Los webhooks pendientes sobreviven reinicios del bridge (persiste en `data/webhooks-pending.json`).

Para sincronizar Laravel con webhooks que se atrasaron, consume el endpoint de sync log:
```php
use App\Services\ContpaqiBridgeClient;
$client = app(ContpaqiBridgeClient::class);
$log = $client->pullSyncLog(100);
foreach ($log as $entry) {
    // Procesa eventos no recibidos por tu webhook
}
```

## 🛡️ Seguridad

- **API Key** solo en header `X-Api-Key` (NO en URL ni en body)
- **HTTPS obligatorio** en producción (Let's Encrypt ya configurado)
- **HMAC en webhooks** evita que cualquiera mande payloads falsos a tu Laravel
- **Cloudflare proxy** oculta la IP real del VPS

## ⏰ Programar sync (routes/console.php)

```php
use Illuminate\Support\Facades\Schedule;

Schedule::command('contpaqi:sync clientes --empresa=adJESUS_LOPEZ_NORIEGA --desde="-1 hour"')
    ->everyFiveMinutes()
    ->withoutOverlapping();
```

## 📊 Endpoints del bridge que consume Laravel

| Endpoint | Método | Propósito |
|----------|--------|-----------|
| `/api/Sync/clientes` | GET | Snapshot completo de clientes |
| `/api/Sync/clientes/modificados?desde=YYYY-MM-DD` | GET | Clientes modificados desde fecha |
| `/api/Sync/productos` | GET | Snapshot completo de productos |
| `/api/Sync/productos/modificados?desde=YYYY-MM-DD` | GET | Productos modificados |
| `/api/Sync/documentos/modificados?desde=YYYY-MM-DD` | GET | Documentos modificados |
| `/api/Reportes/ventas?desde=&hasta=` | GET | Reporte de ventas |
| `/api/Sync/log?ultimas=100` | GET | Log de eventos del bridge |
| `/api/Webhooks` | GET/POST | Listar/registrar webhooks |
| `/api/Documentos/factura` | POST | Crear factura |
| `/api/Documentos/timbrar` | POST | Timbrar |
| `/api/Documentos/validar` | POST | Validar antes de timbrar |