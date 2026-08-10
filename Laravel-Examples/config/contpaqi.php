<?php
/**
 * Configuración del bridge CONTPAQi.
 * Colocar en: config/contpaqi.php
 *
 * Uso en cualquier archivo:
 *   $url = config('contpaqi.bridge_url');
 *   $key = config('contpaqi.api_key');
 */

return [
    /*
    |--------------------------------------------------------------------------
    | URL del Bridge
    |--------------------------------------------------------------------------
    | Si estás en la misma LAN: 'http://192.168.1.100:5000'
    | Si usas ZeroTier: 'http://192.168.191.226:5000'
    | Si es un VPS con IP pública: 'https://bridge.miempresa.com'
    */
    'bridge_url' => env('CONTPAQI_BRIDGE_URL', 'http://localhost:5000'),

    /*
    |--------------------------------------------------------------------------
    | API Key
    |--------------------------------------------------------------------------
    | Configurar en .env: CONTPAQI_API_KEY=tu_clave_secreta
    | (la misma que configuraste en appsettings.json del bridge)
    */
    'api_key' => env('CONTPAQI_API_KEY', ''),

    /*
    |--------------------------------------------------------------------------
    | Ruta de la empresa CONTPAQi
    |--------------------------------------------------------------------------
    | Ejemplo Windows: 'C:\\Compac\\Empresas\\adMI_EMPRESA'
    */
    'empresa_path' => env('CONTPAQI_EMPRESA_PATH', 'C:\\Compac\\Empresas\\adMI_EMPRESA'),

    /*
    |--------------------------------------------------------------------------
    | Contraseña del CSD (para timbrado)
    |--------------------------------------------------------------------------
    */
    'csd_password' => env('CONTPAQI_CSD_PASSWORD', ''),

    /*
    |--------------------------------------------------------------------------
    | Código de concepto por defecto
    |--------------------------------------------------------------------------
    | "4" = Factura, "3" = Nota de crédito, etc.
    */
    'concepto_factura' => env('CONTPAQI_CONCEPTO_FACTURA', '4'),

    /*
    |--------------------------------------------------------------------------
    | Sync automático
    |--------------------------------------------------------------------------
    | Frecuencia del scheduler. Ver app/Console/Kernel.php
    */
    'sync_interval_minutes' => env('CONTPAQI_SYNC_INTERVAL', 15),
];