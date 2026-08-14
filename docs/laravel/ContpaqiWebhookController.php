<?php
/**
 * ContpaqiBridge Laravel Webhook Receiver
 *
 * Endpoint Laravel que recibe webhooks firmados del bridge de CONTPAQi.
 *
 * Ubicación sugerida: app/Http/Controllers/ContpaqiWebhookController.php
 *
 * Headers que envía el bridge:
 *   X-Contpaqi-Signature: sha256=<hex>
 *   X-Contpaqi-Event: timbrado.exitoso | timbrado.fallido | cancelacion.* | factura.creada | *
 *   X-Contpaqi-Delivery: <uuid de la entrega>
 *   X-Contpaqi-Attempt: 1..4
 *
 * Body (JSON):
 *   { "evento": "...", "timestamp": "ISO8601", "payload": { ... } }
 */

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Log;
use Illuminate\Support\Facades\DB;

class ContpaqiWebhookController extends Controller
{
    /**
     * Endpoint único que recibe TODOS los webhooks.
     * Configurar en routes/webhooks.php (ver archivo de rutas).
     */
    public function handle(Request $request)
    {
        $body = $request->getContent();
        $signature = $request->header('X-Contpaqi-Signature', '');
        $event     = $request->header('X-Contpaqi-Event', '');
        $delivery  = $request->header('X-Contpaqi-Delivery', '');
        $attempt   = (int) $request->header('X-Contpaqi-Attempt', '1');

        // 1. Verificar firma HMAC-SHA256
        $secret = config('contpaqi.webhook_secret');
        if (!$secret) {
            Log::error('Contpaqi webhook secret no configurado');
            return response()->json(['ok' => false, 'error' => 'secret_no_configurado'], 500);
        }

        if (!preg_match('/^sha256=([a-f0-9]{64})$/', $signature, $m)) {
            Log::warning("Contpaqi webhook firma con formato inválido", ['sig' => $signature]);
            return response()->json(['ok' => false, 'error' => 'firma_invalida'], 403);
        }
        $firmaRecibida = $m[1];
        $firmaCalculada = hash_hmac('sha256', $body, $secret);
        if (!hash_equals($firmaCalculada, $firmaRecibida)) {
            Log::warning("Contpaqi webhook firma NO coincide (posible spoofing)", [
                'delivery' => $delivery, 'event' => $event, 'attempt' => $attempt,
            ]);
            return response()->json(['ok' => false, 'error' => 'firma_no_coincide'], 403);
        }

        // 2. Parsear payload
        $data = json_decode($body, true);
        if (!is_array($data)) {
            return response()->json(['ok' => false, 'error' => 'json_invalido'], 400);
        }
        $event   = $data['evento']   ?? $event;
        $payload = $data['payload'] ?? [];

        // 3. Despachar al handler según evento
        try {
            switch ($event) {
                case 'timbrado.exitoso':
                    $this->onTimbradoExitoso($payload, $delivery);
                    break;
                case 'timbrado.fallido':
                    $this->onTimbradoFallido($payload, $delivery);
                    break;
                case 'cancelacion.exitosa':
                    $this->onCancelacionExitosa($payload, $delivery);
                    break;
                case 'cancelacion.fallida':
                    $this->onCancelacionFallida($payload, $delivery);
                    break;
                case 'factura.creada':
                    $this->onFacturaCreada($payload, $delivery);
                    break;
                default:
                    Log::info("Contpaqi webhook evento no manejado: $event", $payload);
            }

            // Responder 200 rápido para que el bridge marque como entregado.
            return response()->json(['ok' => true]);
        }
        catch (\Throwable $e) {
            Log::error("Contpaqi webhook handler error: " . $e->getMessage(), [
                'event' => $event, 'delivery' => $delivery, 'exception' => $e,
            ]);
            // Devolver 500 hace que el bridge reintente con backoff (60s, 5min, 15min)
            return response()->json(['ok' => false, 'error' => $e->getMessage()], 500);
        }
    }

    protected function onTimbradoExitoso(array $p, string $delivery): void
    {
        // Actualizar factura local con UUID
        DB::table('facturas')
            ->where('contpaqi_id', $p['id_documento'] ?? null)
            ->update([
                'uuid'        => $p['uuid'] ?? null,
                'xml_path'    => $p['xml_path'] ?? null,
                'timbrada_at' => now(),
                'updated_at'  => now(),
            ]);
    }

    protected function onTimbradoFallido(array $p, string $delivery): void
    {
        DB::table('facturas')
            ->where('contpaqi_id', $p['id_documento'] ?? null)
            ->update([
                'error_timbrado' => $p['error'] ?? 'desconocido',
                'updated_at'     => now(),
            ]);
        // Opcional: notificar al usuario
    }

    protected function onCancelacionExitosa(array $p, string $delivery): void
    {
        DB::table('facturas')
            ->where('contpaqi_id', $p['id_documento'] ?? null)
            ->update([
                'cancelada_at' => now(),
                'acuse'        => $p['acuse'] ?? null,
                'updated_at'   => now(),
            ]);
    }

    protected function onCancelacionFallida(array $p, string $delivery): void
    {
        DB::table('facturas')
            ->where('contpaqi_id', $p['id_documento'] ?? null)
            ->update([
                'error_cancelacion' => $p['error'] ?? 'desconocido',
                'updated_at'        => now(),
            ]);
    }

    protected function onFacturaCreada(array $p, string $delivery): void
    {
        // Insertar/actualizar factura local
        DB::table('facturas')->updateOrInsert(
            ['contpaqi_id' => $p['id_documento'] ?? null],
            [
                'serie'       => $p['serie'] ?? '',
                'folio'       => $p['folio'] ?? 0,
                'cliente'     => $p['cliente'] ?? '',
                'total'       => $p['total'] ?? 0,
                'empresa'     => $p['empresa'] ?? '',
                'created_at'  => now(),
                'updated_at'  => now(),
            ]
        );
    }
}