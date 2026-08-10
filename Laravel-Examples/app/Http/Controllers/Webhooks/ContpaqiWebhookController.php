<?php
/**
 * Controller para recibir webhooks del ContpaqiBridge.
 * Colocar en: app/Http/Controllers/Webhooks/ContpaqiWebhookController.php
 *
 * Ruta: POST /webhooks/contpaqi
 * Registrar en routes/web.php:
 *   Route::post('/webhooks/contpaqi', [ContpaqiWebhookController::class, 'handle']);
 */

namespace App\Http\Controllers\Webhooks;

use App\Http\Controllers\Controller;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Log;

class ContpaqiWebhookController extends Controller
{
    public function handle(Request $request)
    {
        $evento = $request->input('evento');
        $payload = $request->input('payload');
        $timestamp = $request->input('timestamp');

        Log::info("Webhook CONTPAQi recibido: {$evento}", (array) $payload);

        switch ($evento) {
            case 'timbrado.exitoso':
                $this->onTimbradoExitoso($payload);
                break;
            case 'timbrado.fallido':
                $this->onTimbradoFallido($payload);
                break;
            case 'cancelacion.exitosa':
                $this->onCancelacionExitosa($payload);
                break;
            case 'cancelacion.fallida':
                $this->onCancelacionFallida($payload);
                break;
            case 'documento.creado':
                $this->onDocumentoCreado($payload);
                break;
            default:
                Log::warning("Webhook CONTPAQi no manejado: {$evento}");
        }

        return response()->json(['received' => true]);
    }

    /**
     * Cuando el bridge termina de timbrar una factura, llega este evento.
     * Aquí actualizamos nuestra tabla de facturas con el UUID, folio fiscal, etc.
     */
    protected function onTimbradoExitoso(object $payload): void
    {
        DB::table('facturas')->updateOrInsert(
            [
                'serie' => $payload->serie ?? '',
                'folio' => $payload->folio ?? 0
            ],
            [
                'uuid' => $payload->uuid ?? null,
                'timbrada' => true,
                'fecha_timbrado' => now(),
                'updated_at' => now()
            ]
        );
    }

    protected function onTimbradoFallido(object $payload): void
    {
        DB::table('facturas_log')->insert([
            'tipo' => 'timbrado_fallido',
            'serie' => $payload->serie ?? '',
            'folio' => $payload->folio ?? 0,
            'error' => $payload->error ?? '',
            'created_at' => now()
        ]);
    }

    protected function onCancelacionExitosa(object $payload): void
    {
        DB::table('facturas')->updateOrInsert(
            ['serie' => $payload->serie ?? '', 'folio' => $payload->folio ?? 0],
            [
                'cancelado' => true,
                'motivo_cancelacion' => $payload->motivoCancelacion ?? null,
                'uuid_sustitucion' => $payload->uuidSustitucion ?? null,
                'fecha_cancelacion' => now(),
                'updated_at' => now()
            ]
        );
    }

    protected function onCancelacionFallida(object $payload): void
    {
        DB::table('facturas_log')->insert([
            'tipo' => 'cancelacion_fallida',
            'serie' => $payload->serie ?? '',
            'folio' => $payload->folio ?? 0,
            'error' => $payload->error ?? '',
            'created_at' => now()
        ]);
    }

    protected function onDocumentoCreado(object $payload): void
    {
        DB::table('facturas')->updateOrInsert(
            [
                'contpaqi_id' => $payload->idDocumento ?? null,
                'serie' => $payload->serie ?? '',
                'folio' => $payload->folio ?? 0
            ],
            [
                'created_via' => 'contpaqi',
                'updated_at' => now()
            ]
        );
    }
}