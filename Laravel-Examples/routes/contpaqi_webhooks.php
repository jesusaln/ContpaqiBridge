<?php
/**
 * Rutas para webhooks del bridge CONTPAQi.
 * Agregar a: routes/web.php
 */

use App\Http\Controllers\Webhooks\ContpaqiWebhookController;

// Webhook de CONTPAQi (debe ser POST y sin CSRF)
Route::post('/webhooks/contpaqi', [ContpaqiWebhookController::class, 'handle'])
    ->withoutMiddleware([\App\Http\Middleware\VerifyCsrfToken::class]);

/**
 * Ejemplo de uso en un controller de Laravel:
 *
 * public function facturar(Request $request) {
 *     return response()->json(
 *         app(\App\Services\ContpaqiBridgeService::class)->facturar([
 *             'cliente' => [
 *                 'codigo' => $request->cliente_codigo,
 *                 'razonSocial' => $request->cliente_nombre,
 *                 'rfc' => $request->cliente_rfc,
 *                 'regimenFiscal' => '601',
 *                 'usoCFDI' => 'G03',
 *             ],
 *             'producto' => [
 *                 'codigo' => $request->producto_codigo,
 *                 'nombre' => $request->producto_nombre,
 *                 'precio' => $request->producto_precio,
 *                 'claveSAT' => $request->producto_clave_sat,
 *             ],
 *             'factura' => [
 *                 'codigoConcepto' => config('contpaqi.concepto_factura'),
 *                 'cantidad' => 1,
 *                 'passCSD' => config('contpaqi.csd_password'),
 *             ],
 *         ])
 *     );
 * }
 */