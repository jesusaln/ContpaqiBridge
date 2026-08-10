# Laravel ↔ ContpaqiBridge: Ejemplos de Sincronización

Estos archivos muestran cómo integrar tu aplicación Laravel con el **ContpaqiBridge** para tener sincronización bidireccional con CONTPAQi Comercial.

## 📋 Archivos incluidos

| Archivo | Para qué sirve |
|---|---|
| `app/Services/ContpaqiBridgeService.php` | Cliente HTTP para llamar al bridge |
| `app/Console/Commands/ContpaqiSyncCommand.php` | Comando Artisan `contpaqi:sync` que hace push/pull |
| `app/Http/Controllers/Webhooks/ContpaqiWebhookController.php` | Recibe webhooks del bridge |
| `database/migrations/...add_contpaqi_sync_columns.php` | Migración con columnas de tracking |
| `config/contpaqi.php` | Configuración centralizada |
| `routes/contpaqi_webhooks.php` | Rutas para webhooks |

## 🚀 Instalación en tu proyecto Laravel

1. **Variables de entorno** (.env):
```env
CONTPAQI_BRIDGE_URL=http://192.168.191.226:5000
CONTPAQI_API_KEY=tu_clave_secreta_del_appsettings
CONTPAQI_EMPRESA_PATH=C:\Compac\Empresas\adMI_EMPRESA
CONTPAQI_CSD_PASSWORD=password_de_tu_certificado
CONTPAQI_CONCEPTO_FACTURA=4
```

2. **Copia los archivos** a tu proyecto Laravel en las rutas indicadas.

3. **Ejecuta la migración**:
```bash
php artisan migrate
```

4. **Registra la ruta** del webhook en `routes/web.php`:
```php
require __DIR__.'/contpaqi_webhooks.php';
```

5. **Registra el comando** en `app/Console/Kernel.php`:
```php
protected $commands = [
    \App\Console\Commands\ContpaqiSyncCommand::class,
];

protected function schedule(Schedule $schedule)
{
    // Sincronizar cada 15 minutos
    $schedule->command('contpaqi:sync --direction=pull')
        ->everyFifteenMinutes()
        ->withoutOverlapping();

    // Push cada 5 minutos (catálogos modificados en Laravel → CONTPAQi)
    $schedule->command('contpaqi:sync --direction=push --entidades=clientes,productos')
        ->everyFiveMinutes()
        ->withoutOverlapping();
}
```

## 🔄 Comandos disponibles

```bash
# Sincronización completa (snapshot total, más lento)
php artisan contpaqi:sync --full

# Solo pull: CONTPAQi → Laravel (recomendado cada 15 min)
php artisan contpaqi:sync --direction=pull

# Solo push: Laravel → CONTPAQi (cada 5 min)
php artisan contpaqi:sync --direction=push --entidades=clientes,productos

# Bidireccional solo de clientes
php artisan contpaqi:sync --direction=both --entidades=clientes

# Solo facturas nuevas desde CONTPAQi
php artisan contpaqi:sync --direction=pull --entidades=facturas

# Con batch personalizado
php artisan contpaqi:sync --direction=push --batch=500
```

## 🔗 Configurar webhooks en el bridge

Para que el bridge notifique a Laravel en tiempo real:

```bash
# Cuando se timbra una factura exitosamente
curl -X POST http://localhost:5000/api/Webhooks \
  -H "X-Api-Key: TU_CLAVE" \
  -H "Content-Type: application/json" \
  -d '{
    "evento": "timbrado.exitoso",
    "url": "https://tu-laravel.com/webhooks/contpaqi"
  }'

# Cuando se cancela
curl -X POST http://localhost:5000/api/Webhooks \
  -H "X-Api-Key: TU_CLAVE" \
  -H "Content-Type: application/json" \
  -d '{
    "evento": "cancelacion.exitosa",
    "url": "https://tu-laravel.com/webhooks/contpaqi"
  }'

# Para suscribirte a TODOS los eventos
curl -X POST http://localhost:5000/api/Webhooks \
  -H "X-Api-Key: TU_CLAVE" \
  -H "Content-Type: application/json" \
  -d '{
    "evento": "*",
    "url": "https://tu-laravel.com/webhooks/contpaqi"
  }'
```

Eventos disponibles:
- `timbrado.exitoso` — factura timbrada ante el SAT
- `timbrado.fallido` — error al timbrar
- `cancelacion.exitosa` — factura cancelada ante el SAT
- `cancelacion.fallida` — error al cancelar
- `documento.creado` — nuevo documento creado

## 📊 Flujo de sincronización completo

```
                    ┌─────────────────────────┐
                    │  Tu Laravel (MySQL)     │
                    │  - clientes             │
                    │  - productos            │
                    │  - facturas             │
                    └──────────┬──────────────┘
                               │
                  ┌────────────┼────────────┐
                  │            │            │
              push (5min)  pull (15min)  webhooks
                  │            │            │
                  ▼            ▼            ▼
            ┌──────────────────────────────────┐
            │   ContpaqiBridge (.NET)         │
            │   - /api/Sync/*                  │
            │   - /api/Webhooks (registra)     │
            └──────────────┬───────────────────┘
                           │
                           ▼
            ┌──────────────────────────────────┐
            │  CONTPAQi Comercial (SQL Server) │
            │  - admClientes                   │
            │  - admProductos                  │
            │  - admDocumentos                 │
            └──────────────────────────────────┘
```

## 🎯 Casos de uso comunes

### 1. Sincronización inicial (migrar catálogo Laravel → CONTPAQi)

```bash
php artisan contpaqi:sync --direction=push --entidades=clientes,productos --batch=500
```

Repetir hasta que `ok == error == 0` y todos tengan `sync_status='synced'`.

### 2. Catálogo CONTPAQi → Laravel (lectura)

```bash
php artisan contpaqi:sync --direction=pull --entidades=clientes,productos --full
```

### 3. Traer facturas nuevas

```bash
php artisan contpaqi:sync --direction=pull --entidades=facturas
```

### 4. Facturar desde Laravel

```php
$resultado = app(\App\Services\ContpaqiBridgeService::class)->facturar([
    'cliente' => [...],
    'producto' => [...],
    'factura' => [...],
]);

// El bridge:
// 1. Crea/actualiza cliente en CONTPAQi
// 2. Crea/actualiza producto en CONTPAQi
// 3. Crea la factura
// 4. La timbra
// 5. Dispara webhook 'timbrado.exitoso' que actualiza tu tabla Laravel
```

## ⚠️ Consideraciones

1. **Conflictos**: Si modificas un cliente en AMBOS lados entre sincronizaciones, el último gana (push sobrescribe pull). Para evitarlo, usa el campo `updated_at`/`timestamp` y solo actualiza el más reciente.

2. **Volumen**: Para catálogos de más de 5000 registros, ajusta `--batch` y considera ejecutar en horas de menor actividad.

3. **Frecuencia**: 15 minutos es un buen balance. Para sincronización casi-real-time, usa webhooks + scheduler cada 1-2 minutos.

4. **Errores**: El campo `sync_status` puede quedar en `'error'`. Implementa una vista en Laravel para revisar y reintentar los fallidos.